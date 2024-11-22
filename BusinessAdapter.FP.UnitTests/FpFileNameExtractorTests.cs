namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public sealed partial class FpFileNameExtractorTests
{
	[Test]
	public void ExtractFileName_ShouldReturnCorrectFpFileName()
	{
		// Arrange
		var payload = "sample payload";
		var messageDateTime = "1993-01-26T11:03:49Z";
		var creationDate = "1993-01-25T11:03:49Z";
		var parsedFile = new FpPayloadInfo(
			fixture.Data.SenderEIC,
			fixture.Data.ReceiverEIC,
			DateTime.Parse(messageDateTime),
			FpMessageType.Schedule,
			fixture.Data.FahrplanHaendlerTyp);

		var fpMessage = new InboxFpMessage(
			"messageId",
			fixture.Data.SenderParty,
			fixture.Data.ReceiverParty,
			"contentHash",
			System.Text.Encoding.ASCII.GetBytes(payload),
			new FpBDEWProperties(
				"A01",
				"123",
				fixture.Data.FulfillmentDate,
				"subjectPartyId",
				"subjectPartyRole"));


		fixture.Mocks.FpFileParser.Setup(p => p.ParseCompressedPayload(System.Text.Encoding.ASCII.GetBytes(payload)))
			.Returns(parsedFile);

		// Act
		var result = fixture.CreateExtractor().ExtractFileName(fpMessage);

		// Assert
		Assert.That(result.MessageType, Is.EqualTo(FpMessageType.Schedule));
		Assert.That(result.EicNameBilanzkreis, Is.EqualTo(fixture.Data.ReceiverEIC.Code));
		Assert.That(result.EicNameTso, Is.EqualTo(fixture.Data.SenderEIC.Code));
		Assert.That(result.Timestamp, Is.EqualTo(DateTime.Parse(messageDateTime)));
		Assert.That(result.Date, Is.EqualTo(fixture.Data.FulfillmentDate.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)));
		Assert.That(result.Version, Is.EqualTo("123"));
		Assert.That(result.FahrplanHaendlerTyp, Is.EqualTo(fixture.Data.FahrplanHaendlerTyp));
		Assert.That(result.ToFileName(), Is.EqualTo("20241118_FahrplanHaendlerTyp_receiver-eic-code_sender-eic-code_123.xml"));
	}

	[Test]
	public void ExtractFileName_EssConfirmationReport_ShouldReturnCorrectFileName()
	{
		FpFileNameExtractor fileNameExtractor = fixture.CreateExtractorWithFpFileParser();
		var message = new InboxFpMessage("1337",
			new SendingParty("4033872000058", "type"),
			new ReceivingParty("9903025000008", "type"),
			null,
			File.ReadAllBytes("./Parsing/2024-11-13T09_00_56.5778588Z_A07_1.edi.gz"),
			new FpBDEWProperties("A09", "1", "2024-11-13", "", ""));

		var fileName = fileNameExtractor.ExtractFileName(message);

		var fileNameString = fileName.ToFileName();
		Console.WriteLine(fileNameString);
		Assert.That(fileName.Date, Is.EqualTo("20241113"));
		Assert.That(fileNameString, Does.EndWith($"2024-11-13T09-00-54Z.xml"));
		Assert.That(fileNameString, Is.EqualTo("20241113_TPS_11XSWVIERNHEIMVR_10XDE-EON-NETZ-C_001_CNF_2024-11-13T09-00-54Z.xml"));
	}
}