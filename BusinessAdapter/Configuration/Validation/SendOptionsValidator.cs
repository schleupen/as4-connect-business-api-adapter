namespace Schleupen.AS4.BusinessAdapter.Configuration.Validation;

using Microsoft.Extensions.Options;

public class SendOptionsValidator : IValidateOptions<Configuration.SendOptions>
{
	public ValidateOptionsResult Validate(string? name, Configuration.SendOptions options)
	{
		ValidateOptionsResultBuilder builder = new ValidateOptionsResultBuilder();

		builder.AddResult(this.ValidateSend(options));

		return builder.Build();
	}

	private ValidateOptionsResult ValidateSend(SendOptions? sendOptions)
	{
		if (string.IsNullOrEmpty(sendOptions?.Directory))
		{
			return ValidateOptionsResult.Fail("The send directory is not configured.");
		}

		if (!Directory.Exists(sendOptions.Directory))
		{
			return ValidateOptionsResult.Fail($"The send directory {sendOptions.Directory} does not exist.");
		}

		return ValidateOptionsResult.Success;
	}
}