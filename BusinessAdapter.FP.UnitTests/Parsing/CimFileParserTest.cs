namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class CimFileParserTest
{
	[Test]
	public void Parse_ScheduleMessage_GetsParsedCorrectly()
	{
		var message = this.Parse(fixture.TestData.ExampleCimScheduleMessagePath);

		Assert.That(message, Is.Not.Null);
		Assert.That(message!.Content, Is.Not.Empty);
		Assert.That(message.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A01));
		Assert.That(message.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-07T22:00Z/2024-10-08T22:00Z"));
		Assert.That(message.BDEWProperties.BDEWDocumentNo, Is.EqualTo("2"));
		Assert.That(message.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(message.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(message.Sender.Code, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11X0-1111-0619-M"));
	}

	[Test]
	public void Parse_AcknowledgeMessage_GetsParsedCorrectly()
	{
		var fpMessage = this.Parse(fixture.TestData.ExampleCimAcknowledgeMessagePath);

		Assert.That(fpMessage, Is.Not.Null);
		Assert.That(fpMessage!.Content, Is.Not.Empty);
		Assert.That(fpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A17));
		Assert.That(fpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("86"));
		Assert.That(fpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(fpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(fpMessage.Sender.Code, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(fpMessage.Receiver.Code, Is.EqualTo("11Y0-0000-2483-X"));
	}

	[Test]
	public void Parse_AnomalyReport_GetsParsedCorrectly()
	{
		var fpMessage = this.Parse(fixture.TestData.ExampleCimAnomalyReportPath);

		Assert.That(fpMessage, Is.Not.Null);
		Assert.That(fpMessage!.Content, Is.Not.Empty);
		Assert.That(fpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A16));
		Assert.That(fpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-15T22:00Z/2024-10-16T22:00Z"));
		Assert.That(fpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("148"));
		Assert.That(fpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XEN-XIN-NETZ-C"));
		Assert.That(fpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(fpMessage.Sender.Code, Is.EqualTo("10XEN-XIN-NETZ-C"));
		Assert.That(fpMessage.Receiver.Code, Is.EqualTo("11X0-1111-0706-U"));
	}

	[Test]
	[Ignore("No file")] // TODO
	public void Parse_StatusReport_GetsParsedCorrectly()
	{
	}

	[Test]
	[Ignore("No file")] // TODO
	public void ParseFile_ConfirmationReport_GetsParsedCorrectly()
	{
	}

	// TODO Unhappy paths are untested (missing values)...
}