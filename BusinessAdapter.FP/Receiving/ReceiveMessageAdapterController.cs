// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.FP.Configuration;

	public sealed class ReceiveMessageAdapterController(IOptions<EICMapping> eicMapping, ILogger<ReceiveMessageAdapterController> logger) : IReceiveMessageAdapterController
	{
		public async Task ReceiveAvailableMessagesAsync(CancellationToken cancellationToken)
		{
			logger.LogDebug("Receiving of available messages starting.");
		}
	}
}
