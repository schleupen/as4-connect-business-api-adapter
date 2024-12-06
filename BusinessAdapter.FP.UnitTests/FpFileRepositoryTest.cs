namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public partial class FpFileRepositoryTest
{
	private string _testDirectory;

	[SetUp]
	public void SetUp()
	{
		_testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
		Directory.CreateDirectory(_testDirectory);
	}

	[TearDown]
	public void TearDown()
	{
		if (Directory.Exists(_testDirectory))
		{
			Directory.Delete(_testDirectory, true);
		}
	}
	
	[Test]
    public void WriteInboxMessage_FileAlreadyExists_ThrowsFileAlreadyExistException()
    {
        // Arrange
        var repository = fixture.CreateTestObject();
        var fpMessage = CreateTestFpMessage(false);
        var fileName = "existingFile.xml";
        var filePath = Path.Combine(_testDirectory, fileName);

        File.WriteAllText(filePath, "Test content");
        fixture.Mocks.FpFileNameExctractor.Setup(x => x.ExtractFileName(It.IsAny<InboxFpMessage>())).Returns(fixture.Mocks.FileName.Object);
        fixture.Mocks.FileName.Setup(name => name.ToFileName()).Returns(fileName);

        // Act & Assert
        var exception = Assert.Throws<FileAlreadyExistException>(() =>
	        repository.WriteInboxMessage(fpMessage, _testDirectory));
        Assert.That(exception.Message, Does.Contain($"File with the name {filePath} already exist for message {fpMessage.MessageId}"));
    }

    [Test]
    public void WriteInboxMessage_Success_ReturnsFilePath()
    {
        // Arrange
        var repository = fixture.CreateTestObject();
        var fpMessage = CreateTestFpMessage();
        var fileName = "testFile.xml";

        fixture.Mocks.FpFileNameExctractor.Setup(x => x.ExtractFileName(It.IsAny<InboxFpMessage>())).Returns(fixture.Mocks.FileName.Object);
        fixture.Mocks.FileName.Setup(name => name.ToFileName()).Returns(fileName);
        
        // Act
        var result = repository.WriteInboxMessage(fpMessage, _testDirectory);

        // Assert
        var expectedFilePath = Path.Combine(_testDirectory, fileName);
        Assert.That(result, Is.EqualTo(expectedFilePath));
        Assert.That(File.Exists(expectedFilePath), Is.True);
    }
	
	[Test]
	public void GetFilesFrom_NotExistingPath()
	{
		var repository = fixture.CreateTestObject();

		var path = "./notExistingPATH";
		var directoryResult = repository.GetFilesFrom(path);

		Assert.That(directoryResult.ValidFpFiles, Is.Empty);
		Assert.That(directoryResult.DirectoryPath, Is.EqualTo(path));
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "not found", Times.Once());
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, path, Times.Once());
	}

	[Test]
	public void GetFilesFrom_ParserThrowsException()
	{
		fixture.Mocks.FileParserMock.Setup(x => x.ParseFile(It.IsAny<string>()))
			.Throws<InvalidOperationException>();

		var repository = fixture.CreateTestObject();

		string path = ".";
		var filesPathsInDirectory = Directory.GetFiles(path);
		var directoryResult = repository.GetFilesFrom(path);

		Assert.That(directoryResult.ValidFpFiles, Is.Empty);
		Assert.That(directoryResult.DirectoryPath, Is.EqualTo(path));
		Assert.That(directoryResult.DirectoryPath, Is.EqualTo(path));
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "parse", Times.Exactly(filesPathsInDirectory.Length));
	}

	[Test]
	public void GetFilesFrom_ValidFile_ShouldReturn()
	{
		fixture.Mocks.FileParserMock.Setup(x => x.ParseFile(
			It.IsAny<string>())).Returns(
			new FpFile(
				new EIC("1"),
				new EIC("2"),
				Array.Empty<byte>(),
				"fileName",
				"path",
				null!));

		var repository = fixture.CreateTestObject();

		string path = ".";
		var directoryResult = repository.GetFilesFrom(path);
		var filesPathsInDirectory = Directory.GetFiles(path);

		Assert.That(directoryResult.ValidFpFiles, Has.Exactly(filesPathsInDirectory.Length).Items);
		Assert.That(directoryResult.DirectoryPath, Is.EqualTo(path));
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "", Times.Never());
	}
	
	private InboxFpMessage CreateTestFpMessage(bool includePayload = true)
	{
		var payload = CompressString("Test payload content");
		return new InboxFpMessage(
			Guid.NewGuid().ToString(),
			new SendingParty("id", "type"),
			new ReceivingParty("id", "tye"),
			"Test payload content",
			includePayload ? payload : null,
			new FpBDEWProperties(
				"docType", 
				"docNo", 
				"date", 
				"subject", 
				"role")
		);
	}

	private byte[] CompressString(string content)
	{
		using var inputStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
		using var outputStream = new MemoryStream();
		using var gzipStream = new System.IO.Compression.GZipStream(outputStream, System.IO.Compression.CompressionMode.Compress);
		inputStream.CopyTo(gzipStream);
		gzipStream.Close();
		return outputStream.ToArray();
	}
}