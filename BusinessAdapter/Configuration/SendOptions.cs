namespace Schleupen.AS4.BusinessAdapter.Configuration;

public class SendOptions
{
	public int RetryCount { get; set; }

	public int MessageLimitCount { get; set; }

	public string SendDirectory { get; set; }
}