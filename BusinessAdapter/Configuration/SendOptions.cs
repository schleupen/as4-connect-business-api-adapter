namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record SendOptions
{
	public int RetryCount { get; init; } = 3;

	public int ScanInterval { get; init; } = 60;

	public int MessageLimitCount { get; init; } = 1000;

	public string Directory { get; init; } = default!;
}