namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public interface IReceiveStatus
{
	IReadOnlyCollection<FpInboxMessage> SuccessfulMessages { get; }
	IReadOnlyCollection<FailedInboxMessage> FailedMessages { get; }
	int TotalMessageCount { get; }
}