namespace Schleupen.AS4.BusinessAdapter.Configuration.Validation;

using Microsoft.Extensions.Options;

public class ReceiveOptionsValidator : IValidateOptions<Configuration.ReceiveOptions>
{
	public ValidateOptionsResult Validate(string? name, Configuration.ReceiveOptions options)
	{
		ValidateOptionsResultBuilder builder = new ValidateOptionsResultBuilder();

		builder.AddResult(this.ValidateReceive(options));

		return builder.Build();
	}

	private ValidateOptionsResult ValidateReceive(ReceiveOptions? receiveOptions)
	{
		if (string.IsNullOrEmpty(receiveOptions?.Directory))
		{
			return ValidateOptionsResult.Fail("The receive directory is not configured.");
		}

		if (!Directory.Exists(receiveOptions.Directory))
		{
			return ValidateOptionsResult.Fail($"The receive directory {receiveOptions.Directory} does not exist.");
		}


		return ValidateOptionsResult.Success;
	}
}