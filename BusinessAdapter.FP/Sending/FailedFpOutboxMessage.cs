namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

public class FailedFpOutboxMessage : FailedFpMessage<FpOutboxMessage>
{
	public string ErrorCategory { get; }
	public string FilePath { get; }

	public FailedFpOutboxMessage(FpOutboxMessage message, Exception exception) : base(message, exception)
	{
		this.FilePath = message.FilePath;
		this.ErrorCategory = "Sending";
	}

	public FailedFpOutboxMessage(string filePath, Exception exception) : base(null, exception)
	{
		this.FilePath = filePath;
		this.ErrorCategory = "Parsing";
	}
}