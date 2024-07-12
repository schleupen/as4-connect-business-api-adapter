﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Sending
{
	using System.Collections.Immutable;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Polly;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.FP.Gateways;
	using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

	// TODO Test
	public sealed class FpMessageSender(
		IOptions<SendOptions> sendOptions,
		IFpFileRepository fileRepository,
		IFpOutboxMessageAssembler outboxMessageAssembler,
		IBusinessApiGatewayFactory businessApiGatewayFactory,
		ILogger<FpMessageSender> logger)
		: IFpMessageSender
	{
		private readonly SendOptions sendOptions = sendOptions.Value;
		private const uint RetrySleepDurationInSeconds = 10; // TODO Configure in SendOptions as Timespan

		public async Task<SendStatus> SendAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Sending of available messages starting.");

			var directoryResult = fileRepository.GetFilesFrom(sendOptions.Directory);
			var validFpFiles = directoryResult.ValidFpFiles;
			var selectedFilesToSend = validFpFiles.Take(sendOptions.MessageLimitCount);
			var messagesToSend = outboxMessageAssembler.ToFpOutboxMessages(selectedFilesToSend);

			var sendStatus = new SendStatus(directoryResult.TotalFileCount, sendOptions.MessageLimitCount, directoryResult);
			try
			{
				await Policy.Handle<Exception>()
					.WaitAndRetryAsync(
						sendOptions.Retry.Count,
						x => TimeSpan.FromSeconds(RetrySleepDurationInSeconds),
						(ex, ts) =>
						{
							sendStatus.NewIteration();
							messagesToSend = sendStatus.GetUnsentMessages(); // use only unsent/failed message for next iteration
							logger.LogWarning("Error while sending messages");
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

			logger.LogInformation("Sending '{FilesToSendCount}' FP files [RetryIteration: {Iteration}]", messagesToSend.Count, sendStatus.Iteration);

			var messagesBySender = messagesToSend.GroupBy(m => m.Sender);

			foreach (IGrouping<SendingParty, FpOutboxMessage> messagesFromSender in messagesBySender)
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
								sendStatus.AbortedDueToTooManyConnections();
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