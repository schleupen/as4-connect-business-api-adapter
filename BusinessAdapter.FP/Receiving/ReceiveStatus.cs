namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

using Microsoft.Extensions.Logging;

public class ReceiveStatus : IReceiveStatus
{
    public bool AbortedDueToTooManyConnections { get; private set; }
    public int TotalMessageCount => successfulMessages.Count + failedMessages.Count;

    private readonly List<FpInboxMessage> successfulMessages = new();
    private readonly Dictionary<Guid, FailedInboxMessage> failedMessages = new();

    public void AddSuccessfulReceivedMessage(FpInboxMessage message)
    {
        successfulMessages.Add(message);
    }

    public void AddFailedReceivedMessage(FpInboxMessage message, Exception exception)
    {
	    failedMessages[message.MessageId] = new FailedInboxMessage(message, exception);
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
	        logger.LogWarning("Failed to receive message '{Id}' - {Exception} [{Sender} -> {Receiver}]",
		        failedMessage.Value.Message?.MessageId,
		        failedMessage.Value.Exception.Message,
		        failedMessage.Value.Message?.PartyInfo.Sender?.AsKey(),
		        failedMessage.Value.Message?.PartyInfo.Receiver?.AsKey());
        }

        logger.LogInformation("{SuccessfulMessagesCount}/{TotalMessageCount} messages received successful.", successfulMessages.Count, TotalMessageCount);
    }

    public IReadOnlyCollection<FpInboxMessage> SuccessfulMessages => successfulMessages.AsReadOnly();

    public IReadOnlyCollection<FailedInboxMessage> FailedMessages => failedMessages.Values.ToList().AsReadOnly();
}