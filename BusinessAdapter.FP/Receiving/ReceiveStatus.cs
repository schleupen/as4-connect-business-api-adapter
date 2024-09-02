namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;
public class ReceiveStatus
{
    public int SuccessfulMessageCount => successfulMessages.Count;
    public int FailedMessageCount => failedMessages.Count;
    public int TotalNumberOfMessages => successfulMessages.Count + failedMessages.Count;

    private readonly List<FpInboxMessage> successfulMessages = new();
    private readonly List<(FpInboxMessage Message, Exception Exception)> failedMessages = new();

    public void AddSuccessfulReceivedMessage(FpInboxMessage message)
    {
        successfulMessages.Add(message);
    }

    public void AddFailedReceivedMessage(FpInboxMessage message, Exception exception)
    {
        failedMessages.Add((message, exception));
    }

    public IReadOnlyCollection<FpInboxMessage> GetSuccessfulMessages() => successfulMessages.AsReadOnly();

    public IReadOnlyCollection<(FpInboxMessage Message, Exception Exception)> GetFailedMessages() => failedMessages.AsReadOnly();
}