namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Options;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;

public partial class AdapterOptionsValidatorTest
{
	[Test]
	[TestCase(null)]
	[TestCase("")]
	public void Validate_As4ConnectEndpointInvalid_ReturnsInvalid(string? value)
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.As4ConnectEndpoint = value;

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_ValidConfig_ReturnsSuccess()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		AdapterOptionsValidator validator = new AdapterOptionsValidator();
		var result = validator.Validate(null, options);

		Assert.That(result.Succeeded, Is.True);
	}

	[Test]
	public void Validate_SendNotConfigured_ReturnsFailed()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.Send = null;

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_ReceiveNotConfigured_ReturnsFailed()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.Receive = null;

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_MarktpartnerNotConfigured_Emptry_ReturnsFailed()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.Marketpartners = Array.Empty<string>();

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_MarktpartnerNotConfigured_Null_ReturnsFailed()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.Marketpartners = null;

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_ReceiveDirectoryNotExists_ReturnsFailed()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.Receive = options.Receive! with { Directory = "NA" };
		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_SendDirectoryNotExists_ReturnsFailed()
	{
		AdapterOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options.Send = options.Send! with { Directory = "NA" };

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	private static ValidateOptionsResult ExecuteValidation(AdapterOptions options)
	{
		AdapterOptionsValidator validator = new AdapterOptionsValidator();
		var result = validator.Validate(null, options);
		return result;
	}
}