// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving
{
	using System.Net;
	using Polly;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.FP.Gateways;
	using Schleupen.AS4.BusinessAdapter.FP.Configuration;

	public sealed class ReceiveMessageAdapterController(
		ILogger<ReceiveMessageAdapterController> logger,
		IOptions<ReceiveOptions> receiveOptions,
		IOptions<AdapterOptions> adapterOptions,
		IBusinessApiGatewayFactory businessApiGatewayFactory,
		IFpFileRepository fpFileRepo,
		IOptions<EICMapping> eicMapping) : IReceiveMessageAdapterController
	{
		private readonly ReceiveOptions receiveOptions = receiveOptions.Value;

		private const string TooManyRequestsMessage = "A 429 TooManyRequests status code was encountered while receiving the EDIFACT messages which caused the receiving to end before all messages could be received.";

		public async Task ReceiveAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Receiving of available messages starting.");

			string receiveDirectoryPath = receiveOptions.Directory;
			if (string.IsNullOrEmpty(receiveDirectoryPath))
			{
				throw new CatastrophicException("The receive directory is not configured.");
			}

			IReadOnlyCollection<string> ownMarketpartners = adapterOptions.Value.Marketpartners!;
			if (ownMarketpartners.Count == 0)
			{
				throw new CatastrophicException("No valid own market partners were found.");
			}

			Dictionary<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClients = new Dictionary<MessageReceiveInfo, Gateways.IBusinessApiGateway>();
			int successfulMessageCount = 0;
			int failedMessageCount = 0;
			bool hasTooManyRequestsError = false;
			List<Exception> exceptions = new List<Exception>();
			List<string> marketPartnerWithoutCertificate = new List<string>();

			try
			{
				foreach (string receiverIdentificationNumber in ownMarketpartners)
				{
					exceptions = await QueryMessagesAsync(receiverIdentificationNumber,
						as4BusinessApiClients,
						marketPartnerWithoutCertificate);
				}
				long allAvailableMessageCount = as4BusinessApiClients.Sum(x => x.Key.GetAvailableMessages().Length);

				logger.LogInformation("Receiving {AllAvailableMessageCount} messages.", allAvailableMessageCount);

				foreach (KeyValuePair<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClient in as4BusinessApiClients)
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
				foreach (KeyValuePair<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClient in as4BusinessApiClients)
				{
					as4BusinessApiClient.Value.Dispose();
				}

				int totalNumberOfMessages = as4BusinessApiClients.Keys.Sum(x => x.GetAvailableMessages().Length);
				string statusMessage = CreateSuccessStatusMessage(successfulMessageCount, totalNumberOfMessages, failedMessageCount, marketPartnerWithoutCertificate, hasTooManyRequestsError);
				logger.LogInformation("Receiving available messages finished: {StatusMessage}", statusMessage);
			}
		}

		private async Task<List<Exception>> QueryMessagesAsync(string receiverIdentificationNumber,
			Dictionary<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClients,
			List<string> marketPartnerWithoutCertificate)
		{
			List<Exception> exceptions = new List<Exception>();
			try
			{
				var partyReceiver = eicMapping.Value.GetParty(receiverIdentificationNumber);
				if (partyReceiver == null)
				{
					logger.LogError("Receiving party {partyReceiver} mapping not configured", partyReceiver);
					throw new InvalidOperationException($"Receiving party {partyReceiver} mapping not configured");

				}
				IBusinessApiGateway gateway =
					businessApiGatewayFactory.CreateGateway(
						partyReceiver);
				int messageLimit = receiveOptions.MessageLimitCount;
				MessageReceiveInfo receiveInfo = await gateway.QueryAvailableMessagesAsync(messageLimit);
				as4BusinessApiClients.Add(receiveInfo, gateway);
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

			return exceptions;
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
			KeyValuePair<MessageReceiveInfo, IBusinessApiGateway> receiveContext,
			long allAvailableMessageCount,
			int successfulMessageCountBase,
			int failedMessageCountBase,
			CancellationToken cancellationToken)
		{
			int configuredLimit = this.receiveOptions.MessageLimitCount;
			int messageLimit = Math.Min(receiveContext.Key.GetAvailableMessages().Length, configuredLimit);
			As4FpMessage[] availableMessages = receiveContext.Key.GetAvailableMessages();

			return await Policy.Handle<Exception>()
				.WaitAndRetryAsync(receiveOptions.Retry.Count, _ => TimeSpan.FromSeconds(10), (ex, _) => { logger.LogError(ex, "Error while receiving messages"); })
				.ExecuteAndCaptureAsync(async () =>
				{
					List<As4FpMessage> messagesForRetry = new List<As4FpMessage>();
					List<Exception> exceptions = new List<Exception>();
					for (int i = 0; i < messageLimit; i++)
					{
						int currentMessageCount = i + 1 + successfulMessageCountBase + failedMessageCountBase;
						logger.LogInformation("Receiving message {CurrentMessageCount}/{AllAvailableMessageCount}", currentMessageCount, allAvailableMessageCount);

						try
						{
							BusinessApiResponse<InboxFpMessage> result = await receiveContext.Value.ReceiveMessageAsync(availableMessages[i]);
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
								string fileName = fpFileRepo.StoreXmlFileTo(result.Message, receiveDirectoryPath);

								BusinessApiResponse<bool> ackResponse = await receiveContext.Value.AcknowledgeReceivedMessageAsync(result.Message);
								if (!ackResponse.WasSuccessful)
								{
									fpFileRepo.DeleteFile(fileName);

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
									continue;
								}

								receiveContext.Key.AddReceivedXmlMessage(result.Message);
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
