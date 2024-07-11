namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record SendOptions
{
	public RetryOption Retry { get; init; } = new();

	public TimeSpan ScanInterval { get; init; } = TimeSpan.FromSeconds(60);

	public int MessageLimitCount { get; init; } = 1000;

	public string Directory { get; init; } = default!;
}