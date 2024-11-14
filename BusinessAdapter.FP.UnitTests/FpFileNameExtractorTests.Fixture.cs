namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using System.Text.Json;
using NUnit.Framework;
using Moq;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Microsoft.Extensions.Options;

public sealed partial class FpFileNameExtractorTests
{
	private Fixture fixture = default!;

	[SetUp]
	public void Setup()
	{
		this.fixture = new Fixture();
	}

	[TearDown]
	public void Dispose()
	{
		fixture = null!;
	}

	private sealed class Fixture
	{
		public Mocks Mocks { get; } = new();

		public TestData Data { get; } = new();

		public Fixture()
		{
		}

		public FpFileNameExtractor CreateExtractor()
		{
			return new FpFileNameExtractor(Mocks.FpFileParser.Object, Mocks.EicMapping.Object);
		}

		public FpFileNameExtractor CreateExtractorWithFpFileParser()
		{
			return new FpFileNameExtractor(new FpFileParser(Mocks.FileSystem.Object, new FpParsedFileValidator()), Mocks.EicMapping.Object);
		}
	}

	private sealed class Mocks
	{
		public Mock<IFileSystemWrapper> FileSystem = new Mock<IFileSystemWrapper>();
		public Mock<IFpFileParser> FpFileParser = new Mock<IFpFileParser>();
		public Mock<IOptions<EICMapping>> EicMapping = new Mock<IOptions<EICMapping>>();
	}

	private sealed class TestData
	{
		public EICMapping SampleEicMapping => JsonSerializer.Deserialize<EICMapping>(File.ReadAllText("./Parsing/EICMapping.json"));
	}
}