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
	public void ParseFile_ESS_ConfirmationReportGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleEssConfirmationReportPath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

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
	public void ParseFile_ESS_ScheduleMessageGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleEssScheduleMessagePath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A01"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A08"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("11X0-1111-0762-I"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("10XDE-AOE-HTCC-C"));
	}

	[Test]
	public void ParseFile_ESS_ScheduleMessageGetsParsed_MissingId_ThrowsException()
	{
		string pathOfFile = fixture.TestData.EssScheduleMessagePathOfWrongFile;

		Assert.Throws<ArgumentException>(() => sut.ParseFile(pathOfFile));
	}

	[Test]
	public void ParseFile_ESS_AnomalyReportGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleEssAnomalyReportPath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A16"));
	}

	[Test]
	public void ParseFile_ESS_AcknowledgeMessageGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleEssAcknowledgeMessagePath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A17"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("1"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-ENBW--HGJL"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XDE-ENBW--HGJL"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11XWEISWELTWS-G"));
	}

	[Test]
	public void ParseFile_ESS_StatusReportGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleEssStatusRequestPath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

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
	public void ParseFile_CIM_ConfirmationReportGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleCimConfirmationReportPath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
	}

	[Test]
	public void ParseFile_CIM_ScheduleMessageGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleCimScheduleMessagePath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

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
	public void ParseFile_CIM_AcknowledgeMessageGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleCimAcknowledgeMessagePath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentType, Is.EqualTo("A17"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWDocumentNo, Is.EqualTo("86"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyId, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(outboundFpMessage.BDEWProperties.BDEWSubjectPartyRole, Is.EqualTo("A04"));
		Assert.That(outboundFpMessage.Sender.Code, Is.EqualTo("10XDE-VE-FRISMK"));
		Assert.That(outboundFpMessage.Receiver.Code, Is.EqualTo("11Y0-0000-2483-X"));
	}

	[Test]
	public void ParseFile_CIM_AnomalyReportGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleCimAnomalyReportPath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

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
	public void ParseFile_CIM_StatusReportGetsParsed_Correctly()
	{
		string pathOfFile = fixture.TestData.ExampleCimStatusRequestPath;

		var outboundFpMessage = sut.ParseFile(pathOfFile);

		Assert.That(outboundFpMessage.Content, Is.Not.Empty);
	}

	[Test]
	public void ParseCompressedPayload_ConfirmationReport_EonToViernheim_ShouldPerseCorrectly()
	{
		var message = sut.ParseCompressedPayload(File.ReadAllBytes(@"C:\Users\andre.erlinghagen\Downloads\2024-11-13T09_00_56.5778588Z_A07_1.edi.gz"));

		Assert.That(message.ValidityDate, Is.EqualTo("2024-11-13T09:00:54Z"));
		Assert.That(message.Sender.Code, Is.EqualTo("10XDE-EON-NETZ-C"));
		Assert.That(message.Receiver.Code, Is.EqualTo("11XSWVIERNHEIMVR"));
		Assert.That(message.CreationDate, Is.EqualTo("2024-11-13T09:00:54Z"));
	}


	// TODO Unhappy paths are untested (missing values)...
}