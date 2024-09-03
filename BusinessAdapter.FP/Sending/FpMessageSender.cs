// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Sending
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Polly;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.FP.Gateways;
	using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

	public sealed class FpMessageSender(
		IOptions<SendOptions> sendOptions,
		IFpFileRepository fileRepository,
		IFpOutboxMessageAssembler outboxMessageAssembler,
		IBusinessApiGatewayFactory businessApiGatewayFactory,
		ILogger<FpMessageSender> logger)
		: IFpMessageSender
	{
		public async Task<SendStatus> SendMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Sending of available messages starting.");

			var directoryResult = fileRepository.GetFilesFrom(sendOptions.Value.Directory);
			var validFpFiles = directoryResult.ValidFpFiles;
			var selectedFilesToSend = validFpFiles.Take(sendOptions.Value.MessageLimitCount);
			var messagesToSend = outboxMessageAssembler.ToFpOutboxMessages(selectedFilesToSend);

			var sendStatus = new SendStatus(directoryResult.TotalFileCount, directoryResult);
			try
			{
				await Policy.Handle<Exception>()
					.WaitAndRetryAsync(
						sendOptions.Value.Retry.Count,
						x => sendOptions.Value.Retry.SleepDuration,
						(ex, ts, r, c) =>
						{
							sendStatus.NewRetry();
							messagesToSend = sendStatus.GetUnsentMessagesForRetry(); // use only unsent/failed message for next iteration
							logger.LogWarning("Error while sending messages - retry {CurrentRetry}/{MaxRetryCount} with '{MessagesToSendCount}' messages is scheduled in '{RetrySleepDuration}' at '{ScheduleTime}'", r, sendOptions.Value.Retry.Count, messagesToSend.Count, ts, DateTime.Now + ts);
						})
					.ExecuteAndCaptureAsync(
						async () =>
						{
							await this.SendFilesAsync(messagesToSend, sendStatus, cancellationToken);
							sendStatus.ThrowIfRetryIsNeeded();
						}
					);
			}
			finally
			{
				sendStatus.LogTo(logger);
			}

			return sendStatus;
		}

		private async Task SendFilesAsync(List<FpOutboxMessage> messagesToSend, SendStatus sendStatus, CancellationToken cancellationToken)
		{
			if (messagesToSend.Count == 0)
			{
				return;
			}

			logger.LogInformation("Sending '{FilesToSendCount}' FP files", messagesToSend.Count);

			var messagesBySender = messagesToSend.GroupBy(m => m.Sender);

			foreach (IGrouping<SendingFpParty, FpOutboxMessage> messagesFromSender in messagesBySender)
			{
				try
				{
					using var bapiGateway = businessApiGatewayFactory.CreateGateway(messagesFromSender.Key);

					foreach (var message in messagesFromSender)
					{
						try
						{
							var response = await bapiGateway.SendMessageAsync(message, cancellationToken);
							if (response.HasTooManyRequestsStatusCode())
							{
								sendStatus.AddBusinessApiResponse(response, logger);
								sendStatus.AbortDueToTooManyConnections();
								return;
							}

							sendStatus.AddBusinessApiResponse(response, logger);
							if (response.WasSuccessful)
							{
								fileRepository.DeleteFile(message.FilePath);
							}
						}
						catch (Exception e)
						{
							sendStatus.AddFailure(message, e, logger);
						}
					}
				}
				catch (Exception ex) when (ex is NoUniqueCertificateException or MissingCertificateException)
				{
					foreach (var message in messagesFromSender)
					{
						sendStatus.AddFailure(message, ex, logger);
					}
				}
			}
		}
	}
}