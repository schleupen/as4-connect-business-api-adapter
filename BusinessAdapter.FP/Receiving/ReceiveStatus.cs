namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class ReceiveStatus : IReceiveStatus
{
	private readonly List<FpInboxMessage> successfulMessages = new();
	private readonly List<FailedInboxMessage> failedMessages = new();

    public int TotalMessageCount => successfulMessages.Count + failedMessages.Count;

    public void AddSuccessfulReceivedMessage(FpInboxMessage message)
    {
        successfulMessages.Add(message);
    }

    public void AddFailedReceivedMessage(FpInboxMessage message, Exception exception)
    {
        failedMessages.Add(new FailedInboxMessage(message, exception));
    }

    public IReadOnlyCollection<FpInboxMessage> SuccessfulMessages => successfulMessages.AsReadOnly();

    public IReadOnlyCollection<FailedInboxMessage> FailedMessages => failedMessages.AsReadOnly();
}