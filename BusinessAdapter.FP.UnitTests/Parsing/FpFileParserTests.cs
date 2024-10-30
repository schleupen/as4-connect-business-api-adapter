namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

[TestFixture]
internal sealed partial class FpFileParserTests
{
    private readonly Fixture fixture = new();
    private FpFileParser sut = default!;
    private IFileSystemWrapper fileSystemWrapperMock = default!;
    private IFpParsedFileValidator fpParsedFileValidator = default!;

    [SetUp]
    public void Setup()
    {
        fileSystemWrapperMock = new FileSystemWrapper();
        fpParsedFileValidator = new FpParsedFileValidator();

		sut = new FpFileParser(fileSystemWrapperMock, fpParsedFileValidator);
    }

    [Test]
    public void FpFileParser_ESS_ConfirmationReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleEssConfirmationReportPath;

        var outboundFpMessage = sut.Parse(pathOfFile);

        string senderId = "0X1001A1001A264";

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A08"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("2"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2001-06-02T22:00Z/2001-06-03T22:00Z"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo(senderId));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A01"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo(senderId));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10X000000000RTEM"));
    }

    [Test]
    public void FpFileParser_ESS_ScheduleMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleEssScheduleMessagePath;

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A01"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
    }

    [Test]
    public void FpFileParser_ESS_ScheduleMessageGetsParsed_MissingId_ThrowsException()
    {
        string pathOfFile = fixture.TestData.EssScheduleMessagePathOfWrongFile;

        Assert.Throws<ArgumentException>(() => sut.Parse(pathOfFile));
    }

    [Test]
    public void FpFileParser_ESS_AnomalyReportGetsParsed_Correctly()
    {
	    string pathOfFile = fixture.TestData.ExampleEssAnomalyReportPath;

		var outboundFpMessage = sut.Parse(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
	    Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A16"));
    }

    [Test]
    public void FpFileParser_ESS_AcknowledgeMessageGetsParsed_Correctly()
    {
	    string pathOfFile = fixture.TestData.ExampleEssAcknowledgeMessagePath;

	    var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A17"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-ENBW--HGJL"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XDE-ENBW--HGJL"));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11XWEISWELTWS-G"));
    }

    [Test]
    public void FpFileParser_ESS_StatusReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleEssStatusRequestPath;

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A59"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
    }

    /// CIM
    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_ConfirmationReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleCimConfirmationReportPath;

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }

    [Test]
    public void FpFileParser_CIM_ScheduleMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleCimScheduleMessagePath;

        var outboundFpMessage = sut.Parse(pathOfFile);
        
        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A01"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-07T22:00Z/2024-10-08T22:00Z"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("2"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XEN-VE-FRISMK"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11X0-1111-0619-M"));
	}

    [Test]
    public void FpFileParser_CIM_AcknowledgeMessageGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleCimAcknowledgeMessagePath;

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A17"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("86"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-VE-FRISMK"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XDE-VE-FRISMK"));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11Y0-0000-2483-X"));
    }

    [Test]
    public void FpFileParser_CIM_AnomalyReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleCimAnomalyReportPath;

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A16"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWFulfillmentDate, Is.EqualTo("2024-10-15T22:00Z/2024-10-16T22:00Z"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("148"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XEN-XIN-NETZ-C"));
        Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
        Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XEN-XIN-NETZ-C"));
        Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11X0-1111-0706-U"));
    }

    [Test]
    [Ignore("No file")] // TODO
    public void FpFileParser_CIM_StatusReportGetsParsed_Correctly()
    {
        string pathOfFile = fixture.TestData.ExampleCimStatusRequestPath;

        var outboundFpMessage = sut.Parse(pathOfFile);

        Assert.That(outboundFpMessage.Content, Is.Not.Empty);
    }


    // TODO Unhappy paths are untested (missing values)...
}