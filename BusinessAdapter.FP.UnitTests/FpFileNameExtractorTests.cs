﻿namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

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
		var validityDate = "1993-01-26T11:03:49Z";
		var creationDate = "1993-01-25T11:03:49Z";
		var parsedFile = new FpPayloadInfo(
			fixture.Data.SenderEIC,
			fixture.Data.ReceiverEIC,
			creationDate,
			validityDate);

		var fpMessage = new InboxFpMessage(
			"messageId",
			fixture.Data.SenderParty,
			fixture.Data.ReceiverParty,
			"contentHash",
			System.Text.Encoding.ASCII.GetBytes(payload),
			new FpBDEWProperties(
				"A01",
				"123",
				"FulfillmentDate",
				"subjectPartyId",
				"subjectPartyRole"));


		List<EICMappingEntry> senderEntry = new List<EICMappingEntry>()
		{
			new()
			{
				Bilanzkreis = "BK-Sender",
				EIC = fixture.Data.SenderEIC.Code,
				FahrplanHaendlerTyp = "PPS",
				MarktpartnerTyp = "BDEW"
			}
		};

		List<EICMappingEntry> receiverEntry = new List<EICMappingEntry>()
		{
			new()
			{
				Bilanzkreis = "BK-Receiver",
				EIC = fixture.Data.ReceiverEIC.Code,
				FahrplanHaendlerTyp = "TPS",
				MarktpartnerTyp = "BDEW"
			}
		};

		fixture.Mocks.FpFileParser.Setup(p => p.ParseCompressedPayload(System.Text.Encoding.ASCII.GetBytes(payload)))
			.Returns(parsedFile);
		fixture.Mocks.EicMapping.Setup(x => x.Value).Returns(new EICMapping()
		{
			{ fixture.Data.ReceiverParty.Id, receiverEntry },
			{ fixture.Data.SenderParty.Id, senderEntry }
		});

		// Act
		var result = fixture.CreateExtractor().ExtractFileName(fpMessage);

		// Assert
		Assert.That(result.MessageType, Is.EqualTo(FpMessageType.Schedule));
		Assert.That(result.EicNameBilanzkreis, Is.EqualTo(senderEntry.First().Bilanzkreis));
		Assert.That(result.EicNameTso, Is.EqualTo(fixture.Data.SenderEIC.Code));
		Assert.That(result.Timestamp, Is.EqualTo(validityDate));
		Assert.That(result.Date, Is.EqualTo(creationDate));
		Assert.That(result.Version, Is.EqualTo("123"));
		Assert.That(result.FahrplanHaendlerTyp, Is.EqualTo(senderEntry.First().FahrplanHaendlerTyp));
		Assert.That(result.ToFileName(), Is.EqualTo("19930125_PPS_BK-Sender_sender-eic-code_123.xml"));
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


		var fileNameString = fileName.ToFileName();
		Console.WriteLine(fileNameString);
		Assert.That(fileName.Date, Is.EqualTo("2024-11-13T09:00:54Z"));
		Assert.That(fileNameString, Does.EndWith($"2024-11-13T09-00-54Z.xml"));
		Assert.That(fileNameString, Is.EqualTo("20241113_TPS_11XSWVIERNHEIMVR_10XDE-EON-NETZ-C__CNF_2024-11-13T09-00-54Z.xml"));
	}
}