using Microsoft.Extensions.Logging;

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class ReceiveStatus(int allAvailableMessageCount)
{
    private readonly List<InboxFpMessage> successfulReceivedMessages = new();
    private readonly Dictionary<string, Tuple<InboxFpMessage, Exception>> failedReceivedMessages = new();
    private int failedMessageCount;
    
    public void AddSuccessfulReceivedMessage(InboxFpMessage message)
    {
        successfulReceivedMessages.Add(message);
    }
    
    public void AddFailure(int failedMessageCount)
    {
	    this.failedMessageCount += failedMessageCount;
    }

    public int FailedMessageCount => failedMessageCount;


	public int SuccessfulMessageCount => this.successfulReceivedMessages.Count;

}