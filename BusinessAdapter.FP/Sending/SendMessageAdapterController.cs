// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Sending
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;

	public sealed class SendMessageAdapterController(IJwtBuilder jwtBuilder,
		ILogger<SendMessageAdapterController> logger)
		: ISendMessageAdapterController
	{

		public async Task SendAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Sending of available messages starting.");
		}
	}
}
