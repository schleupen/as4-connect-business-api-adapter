namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

public interface IFpMessageSender
{
	Task<SendStatus> SendMessagesAsync(CancellationToken cancellationToken);
}