namespace Schleupen.AS4.BusinessAdapter.Configuration;

public record ReceiveOptions
{
	public int RetryCount { get; init; } = 3;

	public int MessageLimitCount { get; init; } = 1000;

	public string Directory { get; init; } = default!;
}