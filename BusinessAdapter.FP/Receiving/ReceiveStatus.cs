using Microsoft.Extensions.Logging;

namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;
public class ReceiveStatus
{
    public int SuccessfulMessages => _successfulMessages.Count;
    public int FailedMessages => _failedMessages.Count;
    private readonly List<FpInboxMessage> _successfulMessages;
    private readonly List<(FpInboxMessage Message, Exception Exception)> _failedMessages;

    public ReceiveStatus()
    {
        _successfulMessages = new List<FpInboxMessage>();
        _failedMessages = new List<(FpInboxMessage, Exception)>();
    }

    public void AddSuccessfulReceivedMessage(FpInboxMessage message)
    {
        _successfulMessages.Add(message);
    }

    public void AddFailedReceivedMessage(FpInboxMessage message, Exception exception)
    {
        _failedMessages.Add((message, exception));
    }

    public IReadOnlyCollection<FpInboxMessage> GetSuccessfulMessages() => _successfulMessages.AsReadOnly();

    public IReadOnlyCollection<(FpInboxMessage Message, Exception Exception)> GetFailedMessages() => _failedMessages.AsReadOnly();
}