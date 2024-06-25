namespace Schleupen.AS4.BusinessAdapter.Configuration.Validation;

using Microsoft.Extensions.Options;

public class AdapterOptionsValidator : IValidateOptions<AdapterOptions>
{
	// TODO: unittests
	public ValidateOptionsResult Validate(string? name, AdapterOptions options)
	{
		ValidateOptionsResultBuilder builder = new ValidateOptionsResultBuilder();

		builder.AddResult(this.ValidateMarketpartner(options));
		builder.AddResult(this.ValidateEndpoint(options));

		builder.AddResult(this.ValidateSend(options.Send));
		builder.AddResult(this.ValidateReceive(options.Receive));
		builder.AddResult(this.ValidateCertificateOptions(options));

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

	private ValidateOptionsResult ValidateReceive(ReceiveOptions? receiveOptions)
	{
		if (string.IsNullOrEmpty(receiveOptions?.Directory))
		{
			return ValidateOptionsResult.Fail("The receive directory is not configured.");
		}

		if (!Directory.Exists(receiveOptions.Directory))
		{
			return ValidateOptionsResult.Fail($"The send directory {receiveOptions.Directory} does not exist.");
		}


		return ValidateOptionsResult.Success;
	}

	private ValidateOptionsResult ValidateCertificateOptions(AdapterOptions receiveOptions)
	{
		return ValidateOptionsResult.Success;
	}

	private ValidateOptionsResult ValidateEndpoint(AdapterOptions receiveOptions)
	{
		if (string.IsNullOrEmpty(receiveOptions.As4ConnectEndpoint))
		{
			return ValidateOptionsResult.Fail("The endpoint for AS4 connect is not configured.");
		}

		return ValidateOptionsResult.Success;
	}

	private ValidateOptionsResult ValidateMarketpartner(AdapterOptions receiveOptions)
	{
		if (receiveOptions.Marketpartners == null || receiveOptions.Marketpartners.Length == 0)
		{
			return ValidateOptionsResult.Fail("No marketpartner configured.");
		}

		return ValidateOptionsResult.Success;
	}
}