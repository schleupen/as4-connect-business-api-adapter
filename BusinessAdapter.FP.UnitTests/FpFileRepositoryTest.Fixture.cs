namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

public partial class FpFileRepositoryTest
{
	private Fixture fixture = default!;

	[SetUp]
	public void Setup()
	{
		fixture = new Fixture();
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

		public FpFileRepository CreateTestObject()
		{
			return new FpFileRepository(Mocks.FileParserMock.Object, Mocks.FpFileNameExctractor.Object, Mocks.LoggerMock.Object);
		}

		public void VerifyLogMessageContainingText(LogLevel logLevel, string text, Times times)
		{
			Mocks.LoggerMock.Verify(x =>
					x.Log(logLevel,
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(text, StringComparison.InvariantCultureIgnoreCase)),
						It.IsAny<Exception?>(),
						((Func<It.IsAnyType, Exception, string>) It.IsAny<object>())!),
				times);
		}
	}

	private sealed class Mocks
	{
		public Mock<IFpFileParser> FileParserMock { get; } = new();
		
		public Mock<IFpFileNameExtractor> FpFileNameExctractor { get; } = new();
		
		public Mock<FpFileName> FileName { get; } = new();

		public Mock<ILogger<FpFileRepository>> LoggerMock { get; } = new();
	}

	private sealed class TestData
	{
		public string TestValue = "dummy";
	}
}