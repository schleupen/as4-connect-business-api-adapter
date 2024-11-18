namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

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
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("2"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2001-06-02T22:00Z/2001-06-03T22:00Z"));
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
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A01"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
	}

	[Test]
	public void Parse_ScheduleMessage_MissingId_ThrowsException()
	{
		Assert.Throws<ArgumentException>(() => this.Parse(fixture.TestData.EssScheduleMessagePathOfWrongFile));
	}

	[Test]
	public void Parse_AnomalyReport_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssAnomalyReportPath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A16"));
	}

	[Test]
	public void Parse_AcknowledgeMessage_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssAcknowledgeMessagePath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A17"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-ENBW--HGJL"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XDE-ENBW--HGJL"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11XWEISWELTWS-G"));
	}

	[Test]
	public void Parse_StatusRequest_GetsParsedCorrectly()
	{
		var outboundFpMessage = this.Parse(fixture.TestData.ExampleEssStatusRequestPath);

		Assert.That(outboundFpMessage, Is.Not.Null);
		Assert.That(outboundFpMessage!.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A59"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
	}

	[Test]
	[TestCase("./Parsing/EssFiles/20241114_SRQ_11XSWSE-DBA-VZRL_10XDE-RWENET---W_CRQ.xml")]
	[TestCase("./Parsing/EssFiles/20241115_SRQ_11XSWSE-DBA-VZRL_10XDE-RWENET---W_CRQ.xml")]
	public void Parse_StatusRequest_WithCRQPostfix_ShouldBeParsedCorrectly(string path)
	{
		var outboundFpMessage = this.Parse(path);

		Assert.That(outboundFpMessage.BDEWProperties.ToMessageType(), Is.EqualTo(FpMessageType.Status));
	}

	[Test]
	public void ParsePayload_ConfirmationReport_ShouldBeParseCorrectly()
	{
		var message = fixture.CreateTestObject().ParsePayload(XDocument.Load(fixture.TestData.EssConfirmationReportPath));

		Assert.That(message.MessageDateTime, Is.EqualTo(DateTime.Parse("2024-11-13T09:00:54Z").ToUniversalTime()));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-EON-NETZ-C"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11XSWVIERNHEIMVR"));
	}

	// TODO Unhappy paths are untested (missing values)...
}