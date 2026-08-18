namespace Schleupen.AS4.BusinessAdapter.Configuration.Validation;

using Microsoft.Extensions.Options;

public class ReceiveOptionsValidator : IValidateOptions<ReceiveOptions>
{
	public ValidateOptionsResult Validate(string? name, ReceiveOptions options)
	{
		ValidateOptionsResultBuilder builder = new();

		builder.AddResult(ValidateReceive(options));

		return builder.Build();
	}

	private ValidateOptionsResult ValidateReceive(ReceiveOptions? receiveOptions)
	{
		if (string.IsNullOrEmpty(receiveOptions?.Directory))
		{
			return ValidateOptionsResult.Fail("The receive directory is not configured.");
		}

		return Directory.Exists(receiveOptions!.Directory)
			? ValidateOptionsResult.Success
			: ValidateOptionsResult.Fail($"The receive directory {receiveOptions.Directory} does not exist.");
	}
}