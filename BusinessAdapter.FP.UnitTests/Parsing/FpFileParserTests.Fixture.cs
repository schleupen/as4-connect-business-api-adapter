namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Parsing;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

[TestFixture]
internal sealed partial class FpFileParserTests
{
	private Fixture fixture = new();

	[SetUp]
	public void Setup()
	{
		this.fixture = new Fixture();
	}

	private sealed class Fixture
	{
		public TestData TestData { get; private set; } = new();

		public FpFileParser CreateTestObject()
		{
			return new FpFileParser(new FileSystemWrapper(), new FpParsedFileValidator());
		}
	}

	private sealed class TestData
	{
		public const string CimFilePath = "./Parsing/CimFiles";
		public const string EssFilePath = "./Parsing/EssFiles";
		public const string InvalidFilePath = "./Parsing/InvalidFiles";
	}

	private static string[] InvalidFiles()
	{
		var result = Directory.GetFiles(TestData.InvalidFilePath);
		if (result.Length == 0)
		{
			throw new InvalidOperationException($"found files in '{TestData.InvalidFilePath}'");
		}

		return result;
	}

	private static string[] ValidFiles()
	{
		var cimFiles = Directory.GetFiles(TestData.CimFilePath, "*.xml", SearchOption.TopDirectoryOnly);
		if (cimFiles.Length == 0)
		{
			throw new InvalidOperationException($"found files in '{TestData.CimFilePath}'");
		}

		var essFiles = Directory.GetFiles(TestData.EssFilePath, "*.xml", SearchOption.TopDirectoryOnly);
		if (essFiles.Length == 0)
		{
			throw new InvalidOperationException($"found files in '{TestData.EssFilePath}'");
		}

		return cimFiles.Concat(essFiles).ToArray();
	}
}