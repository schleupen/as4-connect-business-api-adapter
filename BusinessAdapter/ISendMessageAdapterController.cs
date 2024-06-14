// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System.Threading.Tasks;

	public interface ISendMessageAdapterController
	{
		Task SendAvailableMessagesAsync(CancellationToken cancellationToken);
	}

	public interface ISendMessageAdapterControllerFactory
	{
		ISendMessageAdapterController GetSendController(ControllerType controllerType);
	}
}
