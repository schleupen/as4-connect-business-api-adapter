// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Sending
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.FP.Gateways;
	using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

	// TODO Test
	public sealed class SendMessageAdapterController(
		IOptions<SendOptions> sendOptions,
		IFpFileRepository fileRepository,
		IFpOutboxMessageAssembler outboxMessageAssembler,
		IBusinessApiGatewayFactory businessApiGatewayFactory,
		ILogger<SendMessageAdapterController> logger)
		: ISendMessageAdapterController
	{
		private readonly SendOptions sendOptions = sendOptions.Value;
		private const uint SendTimeoutInSeconds = 10; // TODO configuration value

		public async Task SendAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Sending of available messages starting.");

			var filesInSendDirectory = await fileRepository.GetFilesFromAsync(sendOptions.Directory, cancellationToken);
			var filesInDirectoryCount = filesInSendDirectory.Count;
			filesInSendDirectory = filesInSendDirectory.Take(sendOptions.MessageLimitCount).ToList();
			var messagesToSend = outboxMessageAssembler.ToFpOutboxMessages(filesInSendDirectory);

			var sendStatus = new SendStatus(filesInDirectoryCount, sendOptions.MessageLimitCount);

			try
			{
				await this.SendFilesAsync(messagesToSend, sendStatus, cancellationToken);
			}
			finally
			{
				sendStatus.LogTo(logger);
			}
		}

		private async Task SendFilesAsync(List<FpOutboxMessage> messagesToSend, SendStatus sendStatus, CancellationToken cancellationToken)
		{
			logger.LogInformation("Sending '{FilesToSendCount}' FP files...", messagesToSend.Count);

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
								await fileRepository.DeleteFileAsync(message.FilePath);
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