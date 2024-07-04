namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

[TestFixture]
internal sealed partial class FpFileParserTests
{
    private readonly Fixture fixture = new();
    private FpFileParser sut = default!;
    private IFileSystemWrapper fileSystemWrapperMock = default!;

    [SetUp]
    public void Setup()
    {
        fileSystemWrapperMock = new FileSystemWrapper();
        sut = new FpFileParser(fileSystemWrapperMock);
    }

    [Test]
    public void FpFileParser_ESS_ConfirmationReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSConfirmationReport();

        var outboundFpMessage = sut.Parse(pathOfFile);

        string senderId = "5790000432752";

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWDocumentType, Is.EqualTo("Confirmation"));
        Assert.That(outboundFpMessage.BDEWDocumentNo, Is.EqualTo("zerotro"));
        Assert.That(outboundFpMessage.BDEWFulfillmentDate, Is.EqualTo("2001-06-02T22:00Z/2001-06-03T22:00Z"));
        Assert.That(outboundFpMessage.BDEWSubjectPartyId, Is.EqualTo(senderId));
        Assert.That(outboundFpMessage.BDEWSubjectPartyRole, Is.EqualTo("A01"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo(senderId));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10X000000000RTEM"));
    }

    [Test]
    public void FpFileParser_ESS_ScheduleMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSScheduleMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWDocumentType, Is.EqualTo("Schedule"));
    }

    [Test]
    public void FpFileParser_ESS_AnomalyReportGetsParsed_Correctly()
    {
	    string pathOfFile = fixture.CreateExampleESSAnomalyReport();

	    var outboundFpMessage = sut.Parse(pathOfFile);

	    Assert.That(outboundFpMessage.Content, Is.Not.Empty);
	    Assert.That(outboundFpMessage.BDEWDocumentType, Is.EqualTo("Anomaly"));
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_ESS_AcknowledgeMessageGetsParsed_Correctly()
    {
	    string pathOfFile = fixture.CreateExampleESSAcknowledgeMessage();

	    var outboundFpMessage = sut.Parse(pathOfFile);

	    Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_ESS_StatusReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSStatusRequest();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    /// CIM
    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_ConfirmationReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMConfirmationReport();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_ScheduleMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMScheduleMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_AcknowledgeMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMAcknowledgeMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_AnomalyReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMAnomalyReport();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_StatusReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMStatusRequest();

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    // TODO Unhappy paths are untested (missing values)...
}