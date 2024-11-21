namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using NUnit.Framework;

[TestFixture]
internal sealed partial class EssFileParserTest
{
	[Test]
	public void Parse_ConfirmationReport_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssConfirmationReportPath);

		string senderId = "0X1001A1001A264";

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A08));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("2"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2001-06-03"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo(senderId));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A01"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo(senderId));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10X000000000RTEM"));
	}

	[Test]
	public void Parse_ScheduleMessage_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssScheduleMessagePath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-22"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A01));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
	}

	[Test]
	public void Parse_ScheduleMessage_MissingId_ThrowsException()
	{
		Assert.Throws<ValidationException>(() => this.Parse(fixture.TestData.EssScheduleMessagePathOfWrongFile));
	}

	[Test]
	public void Parse_AnomalyReport_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.AnomalyReportPath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-11-06"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A16));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-VE-TRANSMK"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11XSWHETT-V----V"));
	}

	[Test]
	public void Parse_AcknowledgeMessage_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssAcknowledgeMessagePath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A17));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-ENBW--HGJL"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-18"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11XWEISWELTWS-G"));
	}

	[Test]
	public void Parse_StatusRequest_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssStatusRequestPath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo(BDEWDocumentTypes.A59));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-22"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
	}

	[Test]
	[TestCase("./Parsing/EssFiles/20241114_SRQ_11XSWSE-DBA-VZRL_10XDE-RWENET---W_CRQ.xml")]
	[TestCase("./Parsing/EssFiles/20241115_SRQ_11XSWSE-DBA-VZRL_10XDE-RWENET---W_CRQ.xml")]
	public void Parse_StatusRequest_WithCRQPostfix_ShouldBeParsedCorrectly(string path)
	{
		var outboundFpMessage = this.Parse(path);

		Assert.That(outboundFpMessage.BDEWProperties.ToMessageType(), Is.EqualTo(FpMessageType.StatusRequest));
	}

	[Test]
	public void ParsePayload_ConfirmationReport_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.EssConfirmationReportPath);

		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-11-13T09:00:54Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-EON-NETZ-C"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11XSWVIERNHEIMVR"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.ConfirmationReport));
	}

	[Test]
	public void ParsePayload_Schedule_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.ExampleEssScheduleMessagePath);

		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-10-25T10:00:19Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(message.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.Schedule));
	}

	[Test]
	public void ParsePayload_StatusRequest_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.ExampleEssStatusRequestPath);

		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-10-25T10:00:30Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(message.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("SRQ"));
		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.StatusRequest));
	}

	[Test]
	public void ParsePayload_Acknowledge_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.ExampleEssAcknowledgeMessagePath);

		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-10-16T10:46:50Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-ENBW--HGJL"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11XWEISWELTWS-G"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));

	}

	[Test]
	public void ParsePayload_AnomalyReport_ShouldBeParseCorrectly()
	{
		var message = ParsePayload(fixture.TestData.AnomalyReportPath);

		Assert.That(message.MessageType, Is.EqualTo(FpMessageType.AnomalyReport));
		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-11-05T08:10:46Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-VE-TRANSMK"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11XSWHETT-V----V"));
		Assert.That(message.FahrplanHaendlerTyp, Is.EqualTo("TPS"));
	}

	[Test]
	public void ParsePayload_AnomalyReportV41_ShouldThrow()
	{
		Assert.Throws<ValidationException>(() => fixture.CreateTestObject().ParsePayload(XDocument.Load(fixture.TestData.AnomalyReportV41Path)));
	}

	// TODO Unhappy paths are untested (missing values)...
}