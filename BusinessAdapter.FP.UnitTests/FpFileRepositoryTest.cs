namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

public partial class FpFileRepositoryTest
{
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
		fixture.Mocks.FileParserMock.Setup(x => x.Parse(It.IsAny<string>()))
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
		fixture.Mocks.FileParserMock.Setup(x => x.Parse(
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
}