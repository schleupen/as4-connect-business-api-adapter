using Microsoft.Extensions.Logging;

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;
public class ReceiveStatus
{
    public int SuccessfulMessageCount => successfulMessages.Count;
    public int FailedMessageCount => failedMessages.Count;
    public int TotalNumberOfMessages => successfulMessages.Count + failedMessages.Count;

    public bool AbortedDueToTooManyConnections { get; private set; }
    
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
            SuccessfulMessageCount);
    }
}