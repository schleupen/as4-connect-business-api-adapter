namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record SendOptions
{
	public string Directory { get; init; } = default!;

	public RetryOption Retry { get; init; } = new();

	public TimeSpan SleepDuration { get; init; } = TimeSpan.FromSeconds(60);

	public int MessageLimitCount { get; init; } = 1000;
}