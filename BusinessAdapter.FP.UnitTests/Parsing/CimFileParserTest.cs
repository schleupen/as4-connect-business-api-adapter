namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class CimFileParserTest
{
	[Test]
	public void Parse_ScheduleMessage_GetsParsedCorrectly()
	{
		var file = this.Parse(fixture.TestData.ScheduleMessagePath);

		Assert.That(file, Is.Not.Null);
		Assert.That(file!.Content, Is.Not.Empty);

		Assert.That(file.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A01));
		Assert.That(file.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-08"));
		Assert.That(file.BDEWProperties.BDEWDocumentNo, Is.EqualTo("2"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(file.Sender.Code, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(file.Receiver.Code, Is.EqualTo("11X0-1111-0619-M"));
	}

	[Test]
	public void Parse_AcknowledgeMessage_GetsParsedCorrectly()
	{
		var file = this.Parse(fixture.TestData.AcknowledgeMessagePath);

		Assert.That(file, Is.Not.Null);
		Assert.That(file!.Content, Is.Not.Empty);
		Assert.That(file.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A17));
		Assert.That(file.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-16"));
		Assert.That(file.BDEWProperties.BDEWDocumentNo, Is.EqualTo("86"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(file.Sender.Code, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(file.Receiver.Code, Is.EqualTo("11Y0-0000-2483-X"));
	}

	[Test]
	public void Parse_AnomalyReport_GetsParsedCorrectly()
	{
		var file = this.Parse(fixture.TestData.AnomalyReportPath);

		Assert.That(file, Is.Not.Null);
		Assert.That(file!.Content, Is.Not.Empty);
		Assert.That(file.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A16));
		Assert.That(file.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-16"));
		Assert.That(file.BDEWProperties.BDEWDocumentNo, Is.EqualTo("148"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XEN-XIN-NETZ-C"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(file.Sender.Code, Is.EqualTo("10XEN-XIN-NETZ-C"));
		Assert.That(file.Receiver.Code, Is.EqualTo("11X0-1111-0706-U"));
	}

	[Test]
	public void Parse_StatusRequest_GetsParsedCorrectly()
	{
		var file = this.Parse(fixture.TestData.StatusRequestPath);

		Assert.That(file, Is.Not.Null);
		Assert.That(file!.Content, Is.Not.Empty);
		Assert.That(file.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A59));
		Assert.That(file.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-11-04"));
		Assert.That(file.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-0000-0619-M"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(file.Sender.Code, Is.EqualTo("11X0-0000-0619-M"));
		Assert.That(file.Receiver.Code, Is.EqualTo("10XDE-VE-TRANSMK"));
	}

	[Test]
	public void Parse_ConfirmationReport_GetsParsedCorrectly()
	{
		var file = this.Parse(fixture.TestData.ConfirmationReportPath);

		Assert.That(file, Is.Not.Null);
		Assert.That(file!.Content, Is.Not.Empty);
		Assert.That(file.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A09));
		Assert.That(file.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-11-04"));
		Assert.That(file.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1337"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-EON-NETZ-C"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(file.Sender.Code, Is.EqualTo(file.BDEWProperties.BDEWSubjectPartyId));
		Assert.That(file.Receiver.Code, Is.EqualTo("11X0-0000-0706-U"));
	}

	// TODO missing ParsePayload tests cases

	// TODO Unhappy paths are untested (missing values)...
}