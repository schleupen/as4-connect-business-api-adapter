namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class FpFileParserTests
{
    private readonly Fixture fixture = new();
    private FpFileParser sut;
    private IFileSystemWrapper fileSystemWrapperMock;

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
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
        Assert.That(outboundFpMessage.CreateOutboxMessage().BDEWDocumentType, Is.EqualTo("Confirmation"));
    }
    
    [Test]
    public void FpFileParser_ESS_ScheduleMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSScheduleMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
        Assert.That(outboundFpMessage.CreateOutboxMessage().BDEWDocumentType, Is.EqualTo("Schedule"));
    }
    
    [Test]
    [Ignore("No file")]
    public void FpFileParser_ESS_AcknowledgeMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSAcknowledgeMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
    
    [Test]
    public void FpFileParser_ESS_AnomalyReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSAnomalyReport();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
        Assert.That(outboundFpMessage.CreateOutboxMessage().BDEWDocumentType, Is.EqualTo("Anomaly"));
    }
       
    [Test]
    [Ignore("No file")]
    public void FpFileParser_ESS_StatusReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleESSStatusRequest();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
    
    /// CIM
    [Test]
    [Ignore("No file")]
    public void FpFileParser_CIM_ConfirmationReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMConfirmationReport();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
    
    [Test]
    [Ignore("No file")]
    public void FpFileParser_CIM_ScheduleMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMScheduleMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
    
    [Test]
    [Ignore("No file")]
    public void FpFileParser_CIM_AcknowledgeMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMAcknowledgeMessage();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
    
    [Test]
    [Ignore("No file")]
    public void FpFileParser_CIM_AnomalyReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMAnomalyReport();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
       
    [Test]
    [Ignore("No file")]
    public void FpFileParser_CIM_StatusReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.CreateExampleCIMStatusRequest();

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.CreateOutboxMessage().Payload, Is.Not.Empty);
    }
}