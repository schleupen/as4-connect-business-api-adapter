namespace Schleupen.AS4.BusinessAdapter;

public interface ISendMessageAdapterControllerFactory
{
	ISendMessageAdapterController GetSendController(ControllerType controllerType);
}