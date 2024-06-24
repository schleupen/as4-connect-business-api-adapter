namespace Schleupen.AS4.BusinessAdapter.Configuration;

public sealed class ReceiveOptions
{
	public int RetryCount { get; set; }

	public int MessageLimitCount { get; set; }

	public string ReceiveDirectory { get; set; }
}