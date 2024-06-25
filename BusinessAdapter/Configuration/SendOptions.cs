namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record SendOptions
{
	public int RetryCount { get; set; }

	public int MessageLimitCount { get; set; }

	public string Directory { get; set; }
}