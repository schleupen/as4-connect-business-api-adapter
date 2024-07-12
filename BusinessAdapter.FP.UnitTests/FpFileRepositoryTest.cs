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

		var files = repository.GetFilesFrom("C:/path");

		Assert.That(files, Is.Empty);
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "not found", Times.Once());
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "C:/path", Times.Once());
	}

	[Test]
	public void GetFilesFrom_ParserThrowsException()
	{
		fixture.Mocks.FileParserMock.Setup(x => x.Parse(It.IsAny<string>()))
			.Throws<InvalidOperationException>();

		var repository = fixture.CreateTestObject();

		var filesPathsInDirectory = Directory.GetFiles(".");
		var files = repository.GetFilesFrom(".");

		Assert.That(files, Is.Empty);
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "parse", Times.Exactly(filesPathsInDirectory.Length));
	}

	[Test]
	public void GetFilesFrom_ValidFile_ShouldReturn()
	{
		fixture.Mocks.FileParserMock.Setup(x => x.Parse(It.IsAny<string>())).Returns(new FpFile(new EIC("1"), new EIC("2"), new byte[0], "fileName", "path", null!));

		var repository = fixture.CreateTestObject();

		var files = repository.GetFilesFrom(".");
		var filesPathsInDirectory = Directory.GetFiles(".");

		Assert.That(files.Count, Is.EqualTo(filesPathsInDirectory.Length));
		fixture.VerifyLogMessageContainingText(LogLevel.Warning, "", Times.Never());
	}
}