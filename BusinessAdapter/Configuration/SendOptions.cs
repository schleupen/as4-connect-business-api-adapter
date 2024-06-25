namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record SendOptions
{
	public int RetryCount { get; set; } = 3;

	public int MessageLimitCount { get; set; } = 1000;

	public string Directory { get; set; }
}