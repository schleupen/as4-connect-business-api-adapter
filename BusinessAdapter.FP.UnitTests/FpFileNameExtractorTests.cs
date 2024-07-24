namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using NUnit.Framework;
using Moq;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFileNameExtractorTests
{
    private Mock<IFpFileParser> _mockParser;
    private FpFileNameExtractor _extractor;

    [SetUp]
    public void Setup()
    {
        _mockParser = new Mock<Schleupen.AS4.BusinessAdapter.FP.Parsing.IFpFileParser>();
        _extractor = new FpFileNameExtractor(_mockParser.Object);
    }

    [Test]
    public void ExtractFileName_ShouldReturnCorrectFpFileName()
    {
        // Arrange
        var payload = "sample payload";
        var receiver = "ReceiverName";
        var validityDate = "26-01-1993";
        var creationDate = "25-01-1993";
        var parsedFile = new FpParsedPayload(
            new EIC(""),
            new EIC(receiver),
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


        _mockParser.Setup(p => p.ParsePayload(System.Text.Encoding.ASCII.GetBytes(payload))).Returns(parsedFile);

        // Act
        var result = _extractor.ExtractFileName(fpMessage);

        // Assert
        Assert.That(result.MessageType, Is.EqualTo(FpMessageType.Schedule));
        Assert.That(result.EicNameBilanzkreis, Is.Empty);
        Assert.That(result.EicNameTso, Is.EqualTo(receiver));
        Assert.That(result.Timestamp, Is.EqualTo(validityDate));
        Assert.That(result.Date, Is.EqualTo(creationDate));
        Assert.That(result.Version, Is.EqualTo("123"));
        Assert.That(result.TypeHaendlerfahrplan, Is.Empty);
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
            .Invoke(_extractor, new object[] { documentType });

        // Assert
        Assert.That(result, Is.EqualTo(expectedMessageType));
    }
}