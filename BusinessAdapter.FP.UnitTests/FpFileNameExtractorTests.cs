namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public sealed partial class FpFileNameExtractorTests
{
	[Test]
	public void ExtractFileName_ShouldReturnCorrectFpFileName()
	{
		// Arrange
		var payload = "sample payload";
		var receiver = "ReceiverNameMpId";
		var receiverEicCode = "eic1";
		var validityDate = "1993-01-26T11:03:49Z";
		var creationDate = "1993-01-25T11:03:49Z";
		var parsedFile = new FpPayloadInfo(
			new EIC("sender"),
			new EIC(receiverEicCode),
			creationDate,
			validityDate);
		var fpMessage = new InboxFpMessage(
			"messageId",
			new SendingParty("sender", "BDEW"),
			new ReceivingParty(receiver, "BDEW"),
			"contentHash",
			System.Text.Encoding.ASCII.GetBytes(payload),
			new FpBDEWProperties(
				"A01",
				"123",
				"FulfillmentDate",
				"subjectPartyId",
				"subjectPartyRole"));


		List<EICMappingEntry> mappedPartyMock = new List<EICMappingEntry>()
		{
			new EICMappingEntry()
			{
				Bilanzkreis = "FINGRID",
				EIC = "sender",
				FahrplanHaendlerTyp = "PPS",
				MarktpartnerTyp = "BDEW"
			}
		};

		fixture.Mocks.FpFileParser.Setup(p => p.ParseCompressedPayload(System.Text.Encoding.ASCII.GetBytes(payload)))
			.Returns(parsedFile);
		fixture.Mocks.EicMapping.Setup(x => x.Value).Returns(new EICMapping()
		{
			{ fpMessage.Receiver.Id, mappedPartyMock }
		});

		// Act
		var result = fixture.CreateExtractor().ExtractFileName(fpMessage);

		// Assert
		Assert.That(result.MessageType, Is.EqualTo(FpMessageType.Schedule));
		Assert.That(result.EicNameBilanzkreis, Is.EqualTo(mappedPartyMock.First().Bilanzkreis));
		Assert.That(result.EicNameTso, Is.EqualTo("sender"));
		Assert.That(result.Timestamp, Is.EqualTo(validityDate));
		Assert.That(result.Date, Is.EqualTo(creationDate));
		Assert.That(result.Version, Is.EqualTo("123"));
		Assert.That(result.FahrplanHaendlerTyp, Is.EqualTo(mappedPartyMock.First().FahrplanHaendlerTyp));
		Assert.That(result.ToFileName(), Is.EqualTo("19930125_PPS_FINGRID_sender_123.xml"));
	}

	[TestCase("A07", FpMessageType.Confirmation)]
	[TestCase("A08", FpMessageType.Confirmation)]
	[TestCase("A09", FpMessageType.Confirmation)]
	[TestCase("A01", FpMessageType.Schedule)]
	[TestCase("A17", FpMessageType.Acknowledge)]
	[TestCase("A16", FpMessageType.Anomaly)]
	[TestCase("A59", FpMessageType.Status)]
	public void ToMessageType_ShouldReturnCorrectMessageType(string documentType, FpMessageType expectedMessageType)
	{
		// Act
		var result = (FpMessageType)typeof(FpFileNameExtractor)
			.GetMethod("ToMessageType",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			.Invoke(fixture.CreateExtractor(), new object[] { documentType });

		// Assert
		Assert.That(result, Is.EqualTo(expectedMessageType));
	}

	[Test]
	public void ExtractFileName_SampleEssFileAndSampleEICMapping_ShouldReturnCorrectFileName()
	{
		fixture.Mocks.EicMapping.Setup(x => x.Value).Returns(fixture.Data.SampleEicMapping);

		FpFileNameExtractor fileNameExtractor = fixture.CreateExtractorWithFpFileParser();
		var message = new InboxFpMessage("1337",
			new SendingParty("4033872000058", "type"),
			new ReceivingParty("9903025000008", "type"),
			null,
			File.ReadAllBytes("./Parsing/2024-11-13T09_00_56.5778588Z_A07_1.edi.gz"),
			new FpBDEWProperties("A09", "", "", "", ""));

		var fileName = fileNameExtractor.ExtractFileName(message);


		Console.WriteLine(fileName.ToFileName());
		Assert.That(fileName.Date, Is.EqualTo("2024-11-13T09:00:54Z"));
		Assert.That(fileName.ToFileName(), Does.EndWith("2024-11-13T09-00-54Z.xml"));
	}
}