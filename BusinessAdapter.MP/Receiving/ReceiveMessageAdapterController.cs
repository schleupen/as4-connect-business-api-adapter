// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Receiving
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Threading.Tasks;
	using BusinessApi;
	using Microsoft.Extensions.Logging;
	using Polly;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.MP.API;
	using Schleupen.AS4.BusinessAdapter.MP.Parsing;

	public sealed class ReceiveMessageAdapterController : IReceiveMessageAdapterController
	{
		private const string TooManyRequestsMessage = "A 429 TooManyRequests status code was encountered while receiving the EDIFACT messages which caused the receiving to end before all messages could be received.";

		private readonly IAs4BusinessApiClientFactory businessApiClientFactory;
		private readonly IConfigurationAccess configuration;
		private readonly IEdifactDirectoryResolver edifactDirectoryResolver;
		private readonly ILogger<ReceiveMessageAdapterController> logger;

		public ReceiveMessageAdapterController(IAs4BusinessApiClientFactory businessApiClientFactory,
			IConfigurationAccess configuration,
			IEdifactDirectoryResolver edifactDirectoryResolver,
			ILogger<ReceiveMessageAdapterController> logger)
		{
			this.businessApiClientFactory = businessApiClientFactory;
			this.configuration = configuration;
			this.edifactDirectoryResolver = edifactDirectoryResolver;
			this.logger = logger;
		}

		public async Task ReceiveAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Receiving of available messages starting.");

			string receiveDirectoryPath = configuration.ReadReceiveDirectory();
			if (string.IsNullOrEmpty(receiveDirectoryPath))
			{
				throw new CatastrophicException("The receive directory is not configured.");
			}

			IReadOnlyCollection<string> ownMarketpartners = configuration.ReadOwnMarketpartners();
			if (ownMarketpartners.Count == 0)
			{
				throw new CatastrophicException("No valid own market partners were found.");
			}

			Dictionary<MessageReceiveInfo, IAs4BusinessApiClient> as4BusinessApiClients = new Dictionary<MessageReceiveInfo, IAs4BusinessApiClient>();
			int successfulMessageCount = 0;
			int failedMessageCount = 0;
			bool hasTooManyRequestsError = false;
			List<Exception> exceptions = new List<Exception>();
			List<string> marketPartnerWithoutCertificate = new List<string>();
			try
			{
				foreach (string receiverIdentificationNumber in ownMarketpartners)
				{
					try
					{
						IAs4BusinessApiClient client = businessApiClientFactory.CreateAs4BusinessApiClient(receiverIdentificationNumber);
						int messageLimit = configuration.ReceivingMessageLimitCount <= 0 ? 1000 : configuration.ReceivingMessageLimitCount;
						MessageReceiveInfo receiveInfo = await client.QueryAvailableMessagesAsync(messageLimit);
						as4BusinessApiClients.Add(receiveInfo, client);
					}
					catch (MissingCertificateException certificateException)
					{
						marketPartnerWithoutCertificate.Add(certificateException.MarketpartnerIdentificationNumber);
					}
					catch (ApiException e)
					{
						logger.LogError("API Exception rethrown: '{Response}'", e.Response);
						throw;
					}
					catch (Exception e)
					{
						exceptions.Add(e);
					}
				}

				long allAvailableMessageCount = as4BusinessApiClients.Sum(x => x.Key.GetAvailableMessages().Length);

				logger.LogInformation("Receiving {AllAvailableMessageCount} messages.", allAvailableMessageCount);

				foreach (KeyValuePair<MessageReceiveInfo, IAs4BusinessApiClient> as4BusinessApiClient in as4BusinessApiClients)
				{
					PolicyResult policyResult = await ReceiveMessagesAsync(receiveDirectoryPath, as4BusinessApiClient, allAvailableMessageCount, successfulMessageCount, failedMessageCount, cancellationToken);
					successfulMessageCount += as4BusinessApiClient.Key.ConfirmableMessages.Count;
					failedMessageCount += as4BusinessApiClient.Key.GetAvailableMessages().Length - as4BusinessApiClient.Key.ConfirmableMessages.Count;
					if (policyResult.FinalException != null)
					{
						exceptions.Add(policyResult.FinalException);
					}

					if (as4BusinessApiClient.Key.HasTooManyRequestsError)
					{
						hasTooManyRequestsError = true;
						break;
					}
				}

				if (exceptions.Count != 0)
				{
					throw new AggregateException("At least one error occured. Details can be found in the inner exceptions.", exceptions);
				}
			}
			finally
			{
				foreach (KeyValuePair<MessageReceiveInfo, IAs4BusinessApiClient> as4BusinessApiClient in as4BusinessApiClients)
				{
					as4BusinessApiClient.Value.Dispose();
				}

				int totalNumberOfMessages = as4BusinessApiClients.Keys.Sum(x => x.GetAvailableMessages().Length);
				string statusMessage = CreateSuccessStatusMessage(successfulMessageCount, totalNumberOfMessages, failedMessageCount, marketPartnerWithoutCertificate, hasTooManyRequestsError);
				logger.LogInformation("Receiving available messages finished: {StatusMessage}", statusMessage);
			}
		}

		private string CreateSuccessStatusMessage(int successfulMessageCount, int totalMessageCount, int failedMessageCount, IReadOnlyCollection<string> marketPartnerWithoutCertificate, bool hasTooManyRequestsError)
		{
			string statusMessage = $"{successfulMessageCount}/{totalMessageCount} messages were received successfully.";
			if (failedMessageCount > 0)
			{
				statusMessage += $" There were {failedMessageCount} messages with errors.";
			}

			if (marketPartnerWithoutCertificate.Count > 0)
			{
				string noCertificateMessage = $" The following market partner does not have a certificate: {string.Join(", ", marketPartnerWithoutCertificate.Distinct())}";
				statusMessage += noCertificateMessage;
			}

			if (hasTooManyRequestsError)
			{
				statusMessage += TooManyRequestsMessage;
			}

			return statusMessage;
		}

		private async Task<PolicyResult> ReceiveMessagesAsync(string receiveDirectoryPath,
			KeyValuePair<MessageReceiveInfo, IAs4BusinessApiClient> receiveContext,
			long allAvailableMessageCount,
			int successfulMessageCountBase,
			int failedMessageCountBase,
			CancellationToken cancellationToken)
		{
			int configuredLimit = configuration.ReceivingMessageLimitCount <= 0 ? int.MaxValue : configuration.ReceivingMessageLimitCount;
			int messageLimit = Math.Min(receiveContext.Key.GetAvailableMessages().Length, configuredLimit);
			MpMessage[] availableMessages = receiveContext.Key.GetAvailableMessages();

			return await Policy.Handle<Exception>()
				.WaitAndRetryAsync(configuration.ReceivingRetryCount, _ => TimeSpan.FromSeconds(10), (ex, _) => { logger.LogError(ex, "Error while receiving messages"); })
				.ExecuteAndCaptureAsync(async () =>
				{
					List<MpMessage> messagesForRetry = new List<MpMessage>();
					List<Exception> exceptions = new List<Exception>();
					for (int i = 0; i < messageLimit; i++)
					{
						int currentMessageCount = i + 1 + successfulMessageCountBase + failedMessageCountBase;
						logger.LogInformation("Receiving message {CurrentMessageCount}/{AllAvailableMessageCount}", currentMessageCount, allAvailableMessageCount);

						try
						{
							MessageResponse<InboxMpMessage> result = await receiveContext.Value.ReceiveMessageAsync(availableMessages[i]);
							if (!result.WasSuccessful)
							{
								if (HandleTooManyRequestError(result.ResponseStatusCode!.Value))
								{
									receiveContext.Key.HasTooManyRequestsError = true;
									return;
								}

								messagesForRetry.Add(availableMessages[i]);
								if (result.ApiException != null)
								{
									exceptions.Add(result.ApiException);
								}
							}
							else
							{
								string fileName;
								try
								{
									fileName = edifactDirectoryResolver.StoreEdifactFileTo(result.Message, receiveDirectoryPath);
								}
								catch (EdifactParsingException ex)
								{
									exceptions.Add(ex);
									continue;
								}

								MessageResponse<bool> ackResponse = await receiveContext.Value.AcknowledgeReceivedMessageAsync(result.Message);
								if (!ackResponse.WasSuccessful)
								{
									edifactDirectoryResolver.DeleteFile(fileName);
									if (HandleTooManyRequestError(ackResponse.ResponseStatusCode!.Value))
									{
										receiveContext.Key.HasTooManyRequestsError = true;
										return;
									}

									messagesForRetry.Add(availableMessages[i]);
									if (ackResponse.ApiException != null)
									{
										exceptions.Add(ackResponse.ApiException);
									}
								}

								receiveContext.Key.AddReceivedEdifactMessage(result.Message);
							}
						}
						catch (Exception ex)
						{
							messagesForRetry.Add(availableMessages[i]);
							exceptions.Add(new RetryableException($"Error while receiving message with identification {availableMessages[i].MessageId}.", ex));
						}
					}

					availableMessages = messagesForRetry.ToArray();
					if (messagesForRetry.Count != 0)
					{
						if (exceptions.Count != 0)
						{
							throw new AggregateException("At least one error occured. Details can be found in the inner exceptions.", exceptions);
						}

						throw new InvalidOperationException("Error while receiving AS4 messages.");
					}
				})
				.WithCancellation(cancellationToken);
		}

		private bool HandleTooManyRequestError(HttpStatusCode httpStatusCode)
		{
			if (httpStatusCode != HttpStatusCode.TooManyRequests)
			{
				return false;
			}

			logger.LogError(TooManyRequestsMessage);
			return true;
		}
	}
}
