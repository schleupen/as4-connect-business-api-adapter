namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class CimFileParserTest
{
	[Test]
	public void Parse_ScheduleMessage_GetsParsedCorrectly()
	{
		var file = Parse(fixture.TestData.ScheduleMessagePath);

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
		var file = Parse(fixture.TestData.AcknowledgeMessagePath);

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
		var file = Parse(fixture.TestData.AnomalyReportPath);

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
		var file = Parse(fixture.TestData.StatusRequestPath);

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
		var file = Parse(fixture.TestData.ConfirmationReportPath);

		Assert.That(file, Is.Not.Null);
		Assert.That(file!.Content, Is.Not.Empty);
		Assert.That(file.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A09));
		Assert.That(file.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-11-04"));
		Assert.That(file.BDEWProperties.BDEWDocumentNo, Is.EqualTo("7"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-EON-NETZ-C"));
		Assert.That(file.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(file.Sender.Code, Is.EqualTo(file.BDEWProperties.BDEWSubjectPartyId));
		Assert.That(file.Receiver.Code, Is.EqualTo("11X0-0000-0706-U"));
	}

	[Test]
	public void ParsePayload_AnomalyReport_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.AnomalyReportPath);

		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.AnomalyReport));
		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-10-16T10:54:41Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XEN-XIN-NETZ-C"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11X0-1111-0706-U"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
	}

	[Test]
	public void ParsePayload_ConfirmationReport_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.ConfirmationReportPath);

		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.ConfirmationReport));
		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-11-03T14:18:21Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-EON-NETZ-C"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11X0-0000-0706-U"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
	}

	[Test]
	public void ParsePayload_Schedule_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.ScheduleMessagePath);

		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.Schedule));
		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-10-07T14:27:06Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11X0-1111-0619-M"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
	}

	[Test]
	public void ParsePayload_StatusRequest_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.StatusRequestPath);

		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.StatusRequest));
		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-11-04T15:05:06Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("11X0-0000-0619-M"));
		Assert.That(message.Receiver.Code, Is.EqualTo("10XDE-VE-TRANSMK"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("SRQ"));
	}

	[Test]
	public void ParsePayload_Acknowledge_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.AcknowledgeMessagePath);

		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.Acknowledge));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11Y0-0000-2483-X"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-10-16T10:48:53Z").ToUniversalTime()));
	}

	// TODO Unhappy paths are untested (missing values)...
}