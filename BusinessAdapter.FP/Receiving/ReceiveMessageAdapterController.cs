// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;

	public sealed class ReceiveMessageAdapterController(ILogger<ReceiveMessageAdapterController> logger) : IReceiveMessageAdapterController
	{
		public async Task ReceiveAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Receiving of available messages starting.");
		}
	}
}
