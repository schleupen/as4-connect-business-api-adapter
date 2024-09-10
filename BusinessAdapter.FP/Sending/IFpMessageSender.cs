namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

public interface IFpMessageSender
{
	Task<ISendStatus> SendMessagesAsync(CancellationToken cancellationToken);
}