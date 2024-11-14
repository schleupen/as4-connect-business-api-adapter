namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using NUnit.Framework;
using Moq;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Microsoft.Extensions.Options;

public class FpFileNameExtractorTests
{
    private Mock<IFpFileParser> mockParser;
    private FpFileNameExtractor extractor;
    private IOptions<EICMapping> eicMapping;

    [SetUp]
    public void Setup()
    {
        mockParser = new Mock<Schleupen.AS4.BusinessAdapter.FP.Parsing.IFpFileParser>();
        eicMapping = Options.Create<EICMapping>(new EICMapping());
        extractor = new FpFileNameExtractor(mockParser.Object, eicMapping);
    }

    [Test]
    public void ExtractFileName_ShouldReturnCorrectFpFileName()
    {
        // Arrange
        var payload = "sample payload";
        var receiver = "ReceiverNameMpId";
        var receiverEicCode = "eic1";
        var validityDate = "26-01-1993";
        var creationDate = "25-01-1993";
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


        List<EICMappingEntry>  mappedPartyMock = new List<EICMappingEntry>()
        {
            new EICMappingEntry()
            {
                Bilanzkreis = "FINGRID",
                EIC = "sender",
                FahrplanHaendlerTyp = "PPS",
                MarktpartnerTyp = "BDEW"
            }
        };
        
        mockParser.Setup(p => p.ParseCompressedPayload(System.Text.Encoding.ASCII.GetBytes(payload))).Returns(parsedFile);
        eicMapping.Value.Add(fpMessage.Receiver.Id, mappedPartyMock);
      
        // Act
        var result = extractor.ExtractFileName(fpMessage);

        // Assert
        Assert.That(result.MessageType, Is.EqualTo(FpMessageType.Schedule));
        Assert.That(result.EicNameBilanzkreis, Is.EqualTo(mappedPartyMock.First().Bilanzkreis));
        Assert.That(result.EicNameTso, Is.EqualTo("sender"));
        Assert.That(result.Timestamp, Is.EqualTo(validityDate));
        Assert.That(result.Date, Is.EqualTo(creationDate));
        Assert.That(result.Version, Is.EqualTo("123"));
        Assert.That(result.FahrplanHaendlerTyp, Is.EqualTo(mappedPartyMock.First().FahrplanHaendlerTyp));
        Assert.That(result.ToFileName(), Is.EqualTo("19930125_PPS_FINGRID_sender_123.XML"));
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
            .Invoke(extractor, new object[] { documentType });

        // Assert
        Assert.That(result, Is.EqualTo(expectedMessageType));
    }
}