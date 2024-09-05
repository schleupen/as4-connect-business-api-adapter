namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FailedInboxMessage(FpInboxMessage message, Exception exception) : FailedFpMessage<FpInboxMessage>(message, exception)
{

}