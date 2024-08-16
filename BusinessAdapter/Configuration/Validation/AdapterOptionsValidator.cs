namespace Schleupen.AS4.BusinessAdapter.Configuration.Validation;

using Microsoft.Extensions.Options;

public class AdapterOptionsValidator : IValidateOptions<Configuration.AdapterOptions>
{
	public ValidateOptionsResult Validate(string? name, Configuration.AdapterOptions options)
	{
		ValidateOptionsResultBuilder builder = new ValidateOptionsResultBuilder();

		builder.AddResult(this.ValidateMarketpartner(options));
		builder.AddResult(this.ValidateEndpoint(options));
		builder.AddResult(this.ValidateCertificateOptions(options));

		return builder.Build();
	}

	private ValidateOptionsResult ValidateCertificateOptions(Configuration.AdapterOptions receiveOptions)
	{
		return ValidateOptionsResult.Success;
	}

	private ValidateOptionsResult ValidateEndpoint(Configuration.AdapterOptions receiveOptions)
	{
		if (string.IsNullOrEmpty(receiveOptions.As4ConnectEndpoint))
		{
			return ValidateOptionsResult.Fail("The endpoint for AS4 connect is not configured.");
		}

		return ValidateOptionsResult.Success;
	}

	private ValidateOptionsResult ValidateMarketpartner(Configuration.AdapterOptions receiveOptions)
	{
		if (receiveOptions.Marketpartners == null || receiveOptions.Marketpartners.Length == 0)
		{
			return ValidateOptionsResult.Fail("No marketpartner configured.");
		}

		return ValidateOptionsResult.Success;
	}
}