namespace Schleupen.AS4.BusinessAdapter.FP;

public class FailedFpMessage<TMessage>(TMessage? message, Exception exception)
{
	public TMessage? Message => message;

	public Exception Exception => exception;
}