namespace Schleupen.AS4.BusinessAdapter.Configuration.Validation;

using Microsoft.Extensions.Options;

public class SendOptionsValidator : IValidateOptions<SendOptions>
{
	public ValidateOptionsResult Validate(string? name, SendOptions options)
	{
		ValidateOptionsResultBuilder builder = new();

		builder.AddResult(ValidateSend(options));

		return builder.Build();
	}

	private ValidateOptionsResult ValidateSend(SendOptions? sendOptions)
	{
		if (string.IsNullOrEmpty(sendOptions?.Directory))
		{
			return ValidateOptionsResult.Fail("The send directory is not configured.");
		}

		return Directory.Exists(sendOptions!.Directory)
			? ValidateOptionsResult.Success
			: ValidateOptionsResult.Fail($"The send directory {sendOptions.Directory} does not exist.");
	}
}