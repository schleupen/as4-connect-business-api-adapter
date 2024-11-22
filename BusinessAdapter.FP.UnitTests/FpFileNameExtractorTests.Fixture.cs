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
			return new FpFileNameExtractor(Mocks.FpFileParser.Object);
		}

		public FpFileNameExtractor CreateExtractorWithFpFileParser()
		{
			return new FpFileNameExtractor(new FpFileParser(Mocks.FileSystem.Object, new FpParsedFileValidator()));
		}
	}

	private sealed class Mocks
	{
		public Mock<IFileSystemWrapper> FileSystem = new Mock<IFileSystemWrapper>();
		public Mock<IFpFileParser> FpFileParser = new Mock<IFpFileParser>();
	}

	private sealed class TestData
	{
		public string FahrplanHaendlerTyp => "FahrplanHaendlerTyp";
		public string FulfillmentDate = "2024-11-18";

		public SendingParty SenderParty = new SendingParty("sender-codenummer", "BDEW");
		public ReceivingParty ReceiverParty = new ReceivingParty("receiver-codenummer", "BDEW");
		public EIC ReceiverEIC = new EIC("receiver-eic-code");
		public EIC SenderEIC = new EIC("sender-eic-code");
	}
}