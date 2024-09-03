namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Options;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;

public partial class SendOptionsValidatorTest
{

	[Test]
	public void Validate_SendNotConfigured_ReturnsFailed()
	{
		var result = ExecuteValidation(null);

		Assert.That(result.Failed, Is.True);
	}

	[Test]
	public void Validate_SendDirectoryNotExists_ReturnsFailed()
	{
		var options = this.fixture.Data.CreateValidAdapterOptions();

		options = options with { Directory = "NA" };

		var result = ExecuteValidation(options);

		Assert.That(result.Failed, Is.True);
	}

	private static ValidateOptionsResult ExecuteValidation(SendOptions options)
	{
		SendOptionsValidator validator = new SendOptionsValidator();
		var result = validator.Validate(null, options);
		return result;
	}
}