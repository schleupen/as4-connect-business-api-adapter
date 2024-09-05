namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

using Microsoft.Extensions.Logging;

public class ReceiveStatus : IReceiveStatus
{
    public bool AbortedDueToTooManyConnections { get; private set; }
    public int TotalMessageCount => successfulMessages.Count + failedMessages.Count;

    private readonly List<FpInboxMessage> successfulMessages = new();
    private readonly List<FailedInboxMessage> failedMessages = new();

    public void AddSuccessfulReceivedMessage(FpInboxMessage message)
    {
        successfulMessages.Add(message);
    }

    public void AddFailedReceivedMessage(FpInboxMessage message, Exception exception)
    {
	    failedMessages.Add(new FailedInboxMessage(message, exception));
    }

    public void AbortDueToTooManyConnections()
    {
        this.AbortedDueToTooManyConnections = true;
    }

    public void LogTo(ILogger logger)
    {
        if (AbortedDueToTooManyConnections)
        {
            logger.LogWarning("A 429 TooManyRequests status code was encountered while receiving the messages which caused the receiving to end before all messages could be received.");
        }

        foreach (var failedMessage in this.failedMessages)
        {
            logger.LogWarning("Failed to receive message for '{MpId}:{MpType}'", failedMessage.Message.PartyInfo.Receiver.Id, failedMessage.Message.PartyInfo.Receiver.Type);
        }

        logger.LogInformation(
            "Messages {SuccessfulMessagesCount} received successful.",
            successfulMessages.Count);
    }

    public IReadOnlyCollection<FpInboxMessage> SuccessfulMessages => successfulMessages.AsReadOnly();

    public IReadOnlyCollection<FailedInboxMessage> FailedMessages => failedMessages.AsReadOnly();
}