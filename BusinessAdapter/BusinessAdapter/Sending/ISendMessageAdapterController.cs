// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using System.Threading.Tasks;

	public interface ISendMessageAdapterController
	{
		Task SendAvailableMessagesAsync(CancellationToken cancellationToken);
	}
}
