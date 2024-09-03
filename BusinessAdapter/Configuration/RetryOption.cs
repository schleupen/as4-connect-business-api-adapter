namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record RetryOption
{
	public int Count { get; init; } = 3;

	public TimeSpan SleepDuration { get; init; } = TimeSpan.FromSeconds(10);
}