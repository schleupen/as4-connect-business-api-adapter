// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Sending
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Polly;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.MP.API;

	public sealed class SendMessageAdapterController : ISendMessageAdapterController
	{
		private readonly IAs4BusinessApiClientFactory businessApiClientFactory;
		private readonly IConfigurationAccess configuration;
		private readonly IEdifactDirectoryResolver edifactDirectoryResolver;
		private readonly ILogger<SendMessageAdapterController> logger;

		public SendMessageAdapterController(
			IAs4BusinessApiClientFactory businessApiClientFactory,
			IConfigurationAccess configuration,
			IEdifactDirectoryResolver edifactDirectoryResolver,
			ILogger<SendMessageAdapterController> logger)
		{
			this.businessApiClientFactory = businessApiClientFactory;
			this.configuration = configuration;
			this.edifactDirectoryResolver = edifactDirectoryResolver;
			this.logger = logger;
		}

		public async Task SendAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			AdapterConfiguration adapterConfiguration = configuration.ReadAdapterConfigurationValue();

			string sendDirectoryPath = configuration.ReadSendDirectory();
			if (string.IsNullOrEmpty(sendDirectoryPath))
			{
				throw new CatastrophicException("The send directory is not configured.");
			}

			List<IEdifactFile> edifactFiles = edifactDirectoryResolver.GetEditfactFilesFrom(sendDirectoryPath).ToList();

			Dictionary<string, IAs4BusinessApiClient> as4BusinessApiClients = new Dictionary<string, IAs4BusinessApiClient>();
			int successfulMessageCount = 0;
			int failedMessageCount = 0;
			int configuredDeliveryLimit = adapterConfiguration.DeliveryMessageLimitCount;
			int deliveryLimit = Math.Min(edifactFiles.Count, configuredDeliveryLimit);
			int initialEdifactFileCount = edifactFiles.Count;
			bool hasTooManyRequestsError = false;

			try
			{
				PolicyResult policyResult = await Policy.Handle<Exception>()
				.WaitAndRetryAsync(adapterConfiguration.DeliveryRetryCount, _ => TimeSpan.FromSeconds(10), (ex, _) => { logger.LogError(ex, "Error while sending messages"); })
				.ExecuteAndCaptureAsync(
					async () =>
					{
						List<IEdifactFile> messagesForRetry = new List<IEdifactFile>();
						List<Exception> exceptions = new List<Exception>();
						for (int i = 0; i < deliveryLimit && i < edifactFiles.Count; i++)
						{
							IEdifactFile edifactFile = edifactFiles[i];
							try
							{
								logger.LogInformation("Sending file {FinishedFiles}/{AllFiles}", i + 1, edifactFiles.Count);
								if (edifactFile.SenderIdentificationNumber == null)
								{
									logger.LogError("Unable to retrieve sender code number from file {Filepath}.", edifactFile.Path);
									continue;
								}

								if (!as4BusinessApiClients.TryGetValue(edifactFile.SenderIdentificationNumber, out IAs4BusinessApiClient? client))
								{
									try
									{
										client = businessApiClientFactory.CreateAs4BusinessApiClient(edifactFile.SenderIdentificationNumber);
										as4BusinessApiClients.Add(edifactFile.SenderIdentificationNumber, client);
									}
									catch (Exception e)
									{
										exceptions.Add(e);
										messagesForRetry.Add(edifactFile);
										continue;
									}
								}

								OutboxMessage outboxMessage = edifactFile.CreateOutboxMessage();
								MessageResponse<OutboxMessage> response = await client.SendMessageAsync(outboxMessage);
								if (!response.WasSuccessful)
								{
									if (response.HasTooManyRequestsStatusCode())
									{
										hasTooManyRequestsError = true;
										return;
									}

									messagesForRetry.Add(edifactFile);
									if (response.ApiException != null)
									{
										exceptions.Add(response.ApiException);
									}
								}
								else
								{
									successfulMessageCount++;
									HandleSuccessfulDelivery(edifactFile);
								}
							}
							catch (Exception e)
							{
								messagesForRetry.Add(edifactFile);
								exceptions.Add(e);
							}
						}

						edifactFiles = messagesForRetry;
						ThrowIfRetryIsNeeded(messagesForRetry, exceptions);
					});

				if (policyResult.FinalException != null)
				{
					failedMessageCount = edifactFiles.Count - successfulMessageCount;
					throw policyResult.FinalException;
				}
			}
			finally
			{
				foreach (KeyValuePair<string, IAs4BusinessApiClient> as4BusinessApiClient in as4BusinessApiClients)
				{
					as4BusinessApiClient.Value.Dispose();
				}

				string statusMessage = CreateStatusMessage(successfulMessageCount, initialEdifactFileCount, failedMessageCount, configuredDeliveryLimit, hasTooManyRequestsError);
				logger.LogInformation("Finished sending available messages: {Status}", statusMessage);
			}
		}

		private static void ThrowIfRetryIsNeeded(ICollection messagesForRetry, IReadOnlyCollection<Exception> exceptions)
		{
			if (messagesForRetry.Count <= 0)
			{
				return;
			}

			if (exceptions.Count != 0)
			{
				throw new AggregateException("There was at least one error. Details can be found in the inner exceptions.", exceptions);
			}

			throw new InvalidOperationException("Error while sending AS4 messages.");
		}

		private string CreateStatusMessage(int successfulMessageCount, int initialEdifactFileCount, int failedMessageCount, int configuredDeliveryLimit, bool hasTooManyRequestsError)
		{
			string statusMessage = $"{successfulMessageCount}/{initialEdifactFileCount} sent successfully.";
			if (failedMessageCount > 0)
			{
				statusMessage += $" There were {failedMessageCount} messages with errors.";
			}

			if (configuredDeliveryLimit < initialEdifactFileCount)
			{
				statusMessage += $" The message limit was set to {configuredDeliveryLimit}.";
			}

			if (hasTooManyRequestsError)
			{
				statusMessage += "A 429 TooManyRequests status code was encountered while sending the EDIFACT messages which caused the sending to end before all messages could be sent.";
			}

			return statusMessage;
		}

		private void HandleSuccessfulDelivery(IEdifactFile edifactFile)
		{
			edifactDirectoryResolver.DeleteFile(edifactFile.Path);
		}
	}
}
