// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Sending
{
	using System.Threading.Tasks;

	public interface IMpMessageSender
	{
		Task SendMessagesAsync(CancellationToken cancellationToken);
	}
}
