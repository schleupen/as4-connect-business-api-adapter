// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Sending
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.FP.Gateways;
	using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

	// TODO Test
	public sealed class SendMessageAdapterController(
		IOptions<SendOptions> sendOptions,
		IFpFileRepository repository,
		IFpOutboxMessageAssembler fpOutboxMessageAssembler,
		IBusinessApiGatewayFactory businessApiGatewayFactory,
		ILogger<SendMessageAdapterController> logger)
		: ISendMessageAdapterController
	{
		private readonly SendOptions sendOptions = sendOptions.Value;
		private const uint SendTimeoutInSeconds = 10; // TODO configuration value

		public async Task SendAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Sending of available messages starting.");

			var filesToSend = await repository.GetFilesFromAsync(sendOptions.Directory, cancellationToken);
			var messagesToSend = fpOutboxMessageAssembler.ToFpOutboxMessages(filesToSend);

			await this.SendFilesAsync(messagesToSend, cancellationToken);
		}

		private async Task SendFilesAsync(List<FpOutboxMessage> messagesToSend, CancellationToken cancellationToken)
		{
			logger.LogInformation("Sending '{FilesToSendCount}' FP files...", messagesToSend.Count);

			var mpIds = messagesToSend.Select(m => m.SenderMessageId).Distinct();

			throw new NotImplementedException();
		}
	}

	public interface IFpFileRepository
	{
		Task<List<FpFile>> GetFilesFromAsync(string sendOptionsDirectory, CancellationToken cancellationToken);
	}
}