namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;

[TestFixture]
internal sealed partial class FpFileParserTests
{
	[Test]
	public void ParseFile_InvalidFile_ThrowsException([ValueSource(nameof(InvalidFiles))] string filePath)
	{
		Assert.That(() => fixture.CreateTestObject().ParseFile(filePath), Throws.Exception);
	}

	[Test]
	public void ParseFile_ValidFile_ShouldNotThrow([ValueSource(nameof(ValidFiles))] string filePath)
	{
		var file = fixture.CreateTestObject().ParseFile(filePath);

		Assert.That(file, Is.Not.Null);
	}
}