namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

public interface ISendStatus
{
	bool AbortedDueToTooManyConnections { get; }
	IReadOnlyCollection<FpOutboxMessage> SuccessfulMessages { get; }
	IReadOnlyCollection<FailedFpOutboxMessage> FailedMessages { get; }
	int RetryIteration { get; }
	int TotalMessageCount { get; }
}