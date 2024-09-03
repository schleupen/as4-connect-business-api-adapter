namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Options;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;

public partial class ReceiveOptionsValidatorTest
{
	[Test]
	public void Validate_ReceiveNotConfigured_ReturnsFailed()
	{
		var result = ExecuteValidation(null);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_ReceiveDirectoryNotExists_ReturnsFailed()
	{
		ReceiveOptions options = this.fixture.Data.CreateValidAdapterOptions();

		options = options with { Directory = "NA" };
		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	private static ValidateOptionsResult ExecuteValidation(ReceiveOptions options)
	{
		ReceiveOptionsValidator validator = new ReceiveOptionsValidator();
		var result = validator.Validate(null, options);
		return result;
	}
}