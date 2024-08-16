namespace Schleupen.AS4.BusinessAdapter.FP.Configuration.Validation;

using Microsoft.Extensions.Options;

public class EICMappingOptionsValidator : IValidateOptions<Configuration.EICMapping>
{
	public ValidateOptionsResult Validate(string? name, EICMapping? options)
	{
		ValidateOptionsResultBuilder builder = new ValidateOptionsResultBuilder();

		if (options is null)
		{
			return ValidateOptionsResult.Fail("Missing EIC-Mapping.");
		}

		if (!options.Any())
		{
			return ValidateOptionsResult.Fail("Empty EIC-Mapping.");
		}

		return builder.Build();
	}
}