// Copyright...:  (c)  Schleupen SE

using Polly;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.FP.Gateways;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving
{
	public sealed class FpMessageReceiver(
		ILogger<FpMessageReceiver> logger,
		IOptions<ReceiveOptions> receiveOptions,
		IOptions<AdapterOptions> adapterOptions,
		IBusinessApiGatewayFactory businessApiGatewayFactory,
		IFpFileRepository fpFileRepo,
		IOptions<EICMapping> eicMapping)
		: IFpMessageReceiver
	{
		private const string TooManyRequestsMessage =
			"A 429 TooManyRequests status code was encountered while receiving the XML messages which caused the receiving to end before all messages could be received.";

		public async Task<IReceiveStatus> ReceiveMessagesAsync(CancellationToken cancellationToken)
		{
			ValidateConfiguration();

			var marketPartnersWithoutCertificate = new List<string>();
			var exceptions = new List<Exception>();

			var as4BusinessApiClients = await QueryAllMessagesAsync(marketPartnersWithoutCertificate, exceptions);

			var receiveStatus = await ProcessAllMessagesAsync(as4BusinessApiClients, exceptions);

			LogFinalStatus(receiveStatus, as4BusinessApiClients, marketPartnersWithoutCertificate);

			receiveStatus.LogTo(logger);

			return receiveStatus;
		}

		private void ValidateConfiguration()
		{
			logger.LogDebug("Receiving of available messages starting.");

			if (string.IsNullOrEmpty(receiveOptions.Value.Directory))
			{
				throw new CatastrophicException("The receive directory is not configured.");
			}

			if (adapterOptions.Value.Marketpartners!.Length == 0)
			{
				throw new CatastrophicException("No valid own market partners were found.");
			}
		}

		private async Task<Dictionary<MessageReceiveInfo, IBusinessApiGateway>> QueryAllMessagesAsync(
			List<string> marketPartnersWithoutCertificate,
			List<Exception> exceptions)
		{
			var as4BusinessApiClients = new Dictionary<MessageReceiveInfo, IBusinessApiGateway>();

			foreach (var receiverIdentificationNumber in adapterOptions.Value.Marketpartners)
			{
				var queryExceptions = await QueryMessagesAsync(receiverIdentificationNumber, as4BusinessApiClients,
					marketPartnersWithoutCertificate);
				exceptions.AddRange(queryExceptions);
			}

			return as4BusinessApiClients;
		}

		private async Task<List<Exception>> QueryMessagesAsync(
			string receiverIdentificationNumber,
			Dictionary<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClients,
			List<string> marketPartnersWithoutCertificate)
		{
			var exceptions = new List<Exception>();
			try
			{
				var partyReceiver = eicMapping.Value.GetPartyOrDefault(receiverIdentificationNumber);
				if (partyReceiver == null)
				{
					var errorMessage = $"Receiving party {receiverIdentificationNumber} mapping not configured";
					logger.LogError(errorMessage);
					throw new InvalidOperationException(errorMessage);
				}

				var gateway = businessApiGatewayFactory.CreateGateway(partyReceiver);
				var messageLimit = receiveOptions.Value.MessageLimitCount;
				var receiveInfo = await gateway.QueryAvailableMessagesAsync(messageLimit);
				as4BusinessApiClients.Add(receiveInfo, gateway);
			}
			catch (MissingCertificateException certificateException)
			{
				marketPartnersWithoutCertificate.Add(certificateException.MarketpartnerIdentificationNumber);
				exceptions.Add(certificateException);
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

		private async Task<ReceiveStatus> ProcessAllMessagesAsync(
			Dictionary<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClients,
			List<Exception> exceptions)
		{
			var receiveStatus = new ReceiveStatus();

			foreach (var as4BusinessApiClient in as4BusinessApiClients)
			{
				var policyResult = await ReceiveMessagesWithRetryAsync(as4BusinessApiClient, receiveStatus);

				if (policyResult.FinalException != null)
				{
					exceptions.Add(policyResult.FinalException);
				}

				if (as4BusinessApiClient.Key.HasTooManyRequestsError)
				{
					break;
				}
			}

			if (exceptions.Count > 0)
			{
				receiveStatus.LogTo(logger);
				throw new AggregateException("At least one error occurred. Details can be found in the inner exceptions.", exceptions);
			}

			return receiveStatus;
		}

		private async Task<PolicyResult> ReceiveMessagesWithRetryAsync(
			KeyValuePair<MessageReceiveInfo, IBusinessApiGateway> receiveContext,
			ReceiveStatus receiveStatus)
		{
			var availableMessages = receiveContext.Key.GetAvailableMessages();
			var messageLimit = Math.Min(availableMessages.Length, receiveOptions.Value.MessageLimitCount);

			return await Policy.Handle<Exception>()
				.WaitAndRetryAsync(receiveOptions.Value.Retry.Count, _ => receiveOptions.Value.Retry.SleepDuration,
					(ex, ts,retryCount,_) =>
					{
						logger.LogWarning("Error while receiving messages - retry {CurrentRetry}/{MaxRetryCount} is scheduled in '{RetrySleepDuration}' at '{ScheduleTime}'",
							retryCount,
							receiveOptions.Value.Retry.Count,
							ts,
							DateTime.Now + ts);
					})
				.ExecuteAndCaptureAsync(async () => { await ReceiveMessagesAsync(receiveContext, availableMessages, messageLimit, receiveStatus); });
		}

		private async Task ReceiveMessagesAsync(
			KeyValuePair<MessageReceiveInfo, IBusinessApiGateway> receiveContext,
			FpInboxMessage[] availableMessages,
			int messageLimit,
			ReceiveStatus receiveStatus)
		{
			var messagesForRetry = new List<FpInboxMessage>();
			var exceptions = new List<Exception>();

			for (var i = 0; i < messageLimit; i++)
			{
				var currentMessage = availableMessages[i];
				try
				{
					await ProcessSingleMessageAsync(receiveContext, currentMessage, receiveStatus);
				}
				catch (Exception ex)
				{
					receiveStatus.AddFailedReceivedMessage(availableMessages[i], ex);
					messagesForRetry.Add(availableMessages[i]);
					exceptions.Add(new RetryableException($"Error while receiving message with identification '{currentMessage.MessageId}'. [{currentMessage.BDEWProperties}]", ex));
				}
			}

			if (messagesForRetry.Count != 0)
			{
				throw new AggregateException(
					"At least one error occurred. Details can be found in the inner exceptions.", exceptions);
			}
		}

		private async Task ProcessSingleMessageAsync(
			KeyValuePair<MessageReceiveInfo, IBusinessApiGateway> receiveContext, FpInboxMessage message,
			ReceiveStatus receiveStatus)
		{
			var result = await receiveContext.Value.ReceiveMessageAsync(message);

			if (!result.WasSuccessful)
			{
				if (HandleTooManyRequestError(result.ResponseStatusCode!.Value))
				{
					receiveContext.Key.HasTooManyRequestsError = true;
					receiveStatus.AbortDueToTooManyConnections();
					return;
				}

				throw new InvalidOperationException("Error while receiving AS4 messages.");
			}

			// Write file
			var fileName = fpFileRepo.WriteInboxMessage(result.Message, receiveOptions.Value.Directory);

			// Ack
			var ackResponse = await receiveContext.Value.AcknowledgeReceivedMessageAsync(result.Message);
			if (!ackResponse.WasSuccessful)
			{
				fpFileRepo.DeleteFile(fileName);

				if (HandleTooManyRequestError(ackResponse.ResponseStatusCode!.Value))
				{
					receiveContext.Key.HasTooManyRequestsError = true;
					return;
				}

				throw new InvalidOperationException($"Error while acknowledging messages with id '{message.MessageId}'.");
			}

			receiveStatus.AddSuccessfulReceivedMessage(message);
		}

		private void LogFinalStatus(ReceiveStatus receiveStatus,
			Dictionary<MessageReceiveInfo, IBusinessApiGateway> as4BusinessApiClients,
			List<string> marketPartnersWithoutCertificate)
		{
			foreach (var as4BusinessApiClient in as4BusinessApiClients)
			{
				as4BusinessApiClient.Value.Dispose();
			}

			var statusMessage = CreateSuccessStatusMessage(
				receiveStatus.SuccessfulMessages.Count,
				receiveStatus.TotalMessageCount,
				receiveStatus.FailedMessages.Count,
				marketPartnersWithoutCertificate,
				as4BusinessApiClients.Any(c => c.Key.HasTooManyRequestsError));

			logger.LogInformation("Receiving available messages finished: {StatusMessage}", statusMessage);
		}

		private string CreateSuccessStatusMessage(
			int successfulMessageCount,
			int totalMessageCount,
			int failedMessageCount,
			IReadOnlyCollection<string> marketPartnersWithoutCertificate,
			bool hasTooManyRequestsError)
		{
			var statusMessage = $"{successfulMessageCount}/{totalMessageCount} messages were received successfully.";
			if (failedMessageCount > 0)
			{
				statusMessage += $" There were {failedMessageCount} messages with errors.";
			}

			if (marketPartnersWithoutCertificate.Count > 0)
			{
				var noCertificateMessage =
					$" The following market partners do not have a certificate: {string.Join(", ", marketPartnersWithoutCertificate.Distinct())}";
				statusMessage += noCertificateMessage;
			}

			if (hasTooManyRequestsError)
			{
				statusMessage += TooManyRequestsMessage;
			}

			return statusMessage;
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