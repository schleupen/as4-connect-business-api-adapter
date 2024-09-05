using Microsoft.IdentityModel.Tokens;

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

		foreach (var keyValuePair in options)
		{
			if (keyValuePair.Value.IsNullOrEmpty())
			{
				return ValidateOptionsResult.Fail("Empty EIC-Mapping.");
			}
			foreach (var mappingEntry in keyValuePair.Value)
			{
				if (mappingEntry.Bilanzkreis.IsNullOrEmpty())
				{
					return ValidateOptionsResult.Fail($"Empty Bilanzkreis for {keyValuePair.Key}.");
				}
				if (mappingEntry.EIC.IsNullOrEmpty())
				{
					return ValidateOptionsResult.Fail($"Empty EIC for {keyValuePair.Key}.");
				}
				if (mappingEntry.FahrplanHaendlerTyp.IsNullOrEmpty())
				{
					return ValidateOptionsResult.Fail($"Empty FahrplanHaendlerTyp for {keyValuePair.Key}.");
				}
				if (mappingEntry.MarktpartnerTyp.IsNullOrEmpty())
				{
					return ValidateOptionsResult.Fail($"Empty MarktpartnerTyp for {keyValuePair.Key}.");
				}
			}
		}
		return builder.Build();
	}
}