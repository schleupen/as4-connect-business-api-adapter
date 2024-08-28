namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending;

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Gateways;
using Schleupen.AS4.BusinessAdapter.FP.Sending;
using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

public partial class FpMessageSenderTest
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

		public void VerifyLoggerContainsMessages(LogLevel expectedLogLevel, string expectedMessage, Times times)
		{
			Func<object, Type, bool> state = (v, _) => v.ToString()!.Contains(expectedMessage, StringComparison.OrdinalIgnoreCase);

			this.Mocks.Logger.Verify(
				x => x.Log(
					It.Is<LogLevel>(l => l == expectedLogLevel),
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => state(v, t)),
					It.IsAny<Exception>(),
					It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), times);
		}

		public FpMessageSender CreateFpMessageSender()
		{
			return new FpMessageSender(Mocks.SendOptions.Object,
				Mocks.FpFileRepository.Object,
				Mocks.FpOutboxMessageAssembler.Object,
				Mocks.BusinessApiGatewayFactory.Object,
				Mocks.Logger.Object);
		}

		public SendOptions CreateSendOptions(int retryCount = 3, int messageLimitCount = Int32.MaxValue)
		{
			return new SendOptions()
			{
				Directory = TestData.SendDir,
				Retry = new RetryOption()
				{
					Count = retryCount,
					SleepDuration = TimeSpan.Zero
				},
				MessageLimitCount = messageLimitCount,
				SleepDuration = TimeSpan.Zero
			};
		}

		public Mock<IBusinessApiGateway> SetupSendingSuccessfulWithLimit(int successfulParsedFiles, int failedParsedFiles, int messageLimit, CancellationToken cancellationToken)
		{
			Mocks.SendOptions
				.Setup(x => x.Value)
				.Returns(CreateSendOptions(3, messageLimit));

			Mocks.FpFileRepository
				.Setup(r => r.GetFilesFrom(TestData.SendDir))
				.Returns(Data.CreateDirectoryResult(successfulParsedFiles, failedParsedFiles));

			Mock<IBusinessApiGateway> gatewayMock = new Mock<IBusinessApiGateway>();

			gatewayMock.Setup(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken))
				.Returns<FpOutboxMessage, CancellationToken>((x, y) => Task.FromResult(new BusinessApiResponse<FpOutboxMessage>(true, x)));

			Mocks.BusinessApiGatewayFactory
				.Setup(r => r.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);

			return gatewayMock;
		}

		public Mock<IBusinessApiGateway> SetupSendingSuccessful(int successfulParsedFiles, int failedParsedFiles, CancellationToken cancellationToken)
		{
			Mocks.SendOptions
				.Setup(x => x.Value)
				.Returns(CreateSendOptions(3));

			Mocks.FpFileRepository
				.Setup(r => r.GetFilesFrom(TestData.SendDir))
				.Returns(Data.CreateDirectoryResult(successfulParsedFiles, failedParsedFiles));

			Mock<IBusinessApiGateway> gatewayMock = new Mock<IBusinessApiGateway>();

			gatewayMock.Setup(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken))
				.Returns<FpOutboxMessage, CancellationToken>((x, y) => Task.FromResult(new BusinessApiResponse<FpOutboxMessage>(true, x)));

			Mocks.BusinessApiGatewayFactory
				.Setup(r => r.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);

			return gatewayMock;
		}
		public Mock<IBusinessApiGateway> SetupMultipleMessagesFromOnlyOneSender(int successfulParsedFiles, int failedParsedFiles,
			CancellationToken cancellationToken)
		{
			Mocks.SendOptions
				.Setup(x => x.Value)
				.Returns(CreateSendOptions(3));

			Mocks.FpFileRepository
				.Setup(r => r.GetFilesFrom(TestData.SendDir))
				.Returns(Data.CreateDirectoryResult(successfulParsedFiles, failedParsedFiles, true));

			Mock<IBusinessApiGateway> gatewayMock = new Mock<IBusinessApiGateway>();

			gatewayMock.Setup(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken))
				.Returns<FpOutboxMessage, CancellationToken>((x, y) => Task.FromResult(new BusinessApiResponse<FpOutboxMessage>(true, x)));

			Mocks.BusinessApiGatewayFactory
				.Setup(r => r.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);

			return gatewayMock;
		}



		public Mock<IBusinessApiGateway> SetupTooManyConnection(CancellationToken cancellationToken)
		{
			Mocks.SendOptions
				.Setup(x => x.Value)
				.Returns(CreateSendOptions(5));

			Mocks.FpFileRepository
				.Setup(r => r.GetFilesFrom(TestData.SendDir))
				.Returns(Data.CreateDirectoryResult(1, 0));

			Mock<IBusinessApiGateway> gatewayMock = new Mock<IBusinessApiGateway>();

			gatewayMock.Setup(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken))
				.Returns<FpOutboxMessage, CancellationToken>((x, y) =>
					Task.FromResult(new BusinessApiResponse<FpOutboxMessage>(false, x, HttpStatusCode.TooManyRequests,
						new InvalidOperationException("..."))));

			Mocks.BusinessApiGatewayFactory
				.Setup(r => r.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);

			return gatewayMock;
		}

		public Mock<IBusinessApiGateway> SetupSendingFailed(int retryCount)
		{
			Mocks.SendOptions
				.Setup(x => x.Value)
				.Returns(CreateSendOptions(retryCount));

			Mocks.FpFileRepository
				.Setup(r => r.GetFilesFrom(TestData.SendDir))
				.Returns(Data.CreateDirectoryResult(1, 0));

			var cancellationToken = new CancellationToken();
			Mock<IBusinessApiGateway> gatewayMock = new Mock<IBusinessApiGateway>();

			gatewayMock.Setup(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken))
				.Returns<FpOutboxMessage, CancellationToken>((x, y) => Task.FromResult(new BusinessApiResponse<FpOutboxMessage>(false, x)));

			Mocks.BusinessApiGatewayFactory
				.Setup(r => r.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);

			return gatewayMock;
		}

		public void SetupEmptyDirectory()
		{
			Mocks.SendOptions
				.Setup(x => x.Value)
				.Returns(CreateSendOptions());

			Mocks.FpFileRepository
				.Setup(r => r.GetFilesFrom(TestData.SendDir))
				.Returns(DirectoryResult.Empty);

			Mocks.FpOutboxMessageAssembler
				.Setup(r => r.ToFpOutboxMessages(It.IsAny<IEnumerable<FpFile>>()))
				.Returns(new List<FpOutboxMessage>());
		}
	}

	private sealed class Mocks
	{
		public Mock<IOptions<SendOptions>> SendOptions { get; } = new();

		public Mock<IFpFileRepository> FpFileRepository { get; } = new();

		public Mock<IFpOutboxMessageAssembler> FpOutboxMessageAssembler { get; } = new();

		public Mock<IBusinessApiGatewayFactory> BusinessApiGatewayFactory { get; } = new();

		public Mock<ILogger<FpMessageSender>> Logger { get; } = new();

		public Mocks()
		{
			FpOutboxMessageAssembler
				.Setup(r => r.ToFpOutboxMessages(It.IsAny<IEnumerable<FpFile>>()))
				.Returns<IEnumerable<FpFile>>(x => new List<FpOutboxMessage>(
					x.Select(x => new FpOutboxMessage(
						Guid.NewGuid(),
						new SendingFpParty(x.Sender.Code, "type", "FpType", "Bilanzkrieis"),
						new ReceivingFpParty(x.Receiver.Code, "type", "FpType", "Bilanzkrieis"),
						x.Content,
						x.FileName,
						x.FilePath,
						x.BDEWProperties))));
		}
	}

	private sealed class TestData
	{
		public const string SendDir = "./Send";

		public DirectoryResult CreateDirectoryResult(int successfulMessages, int failedMessages, bool sameSender = false)
		{
			return new DirectoryResult(
				SendDir,
				CreateFpFiles(successfulMessages, sameSender).ToImmutableList(),
				CreateFailedFpFiles(failedMessages).ToImmutableList());
		}

		private static List<FpFile> CreateFpFiles(int successfulMessages, bool sameSender)
		{
			List<FpFile> files = new List<FpFile>();
			for (int i = 0; i < successfulMessages; i++)
			{
				files.Add(new FpFile(
					sameSender ? new EIC("same") : new EIC(i.ToString()),
					new EIC(i + 1.ToString()),
					Array.Empty<byte>(),
					$"fileName_{i}",
					Path.Combine(SendDir,
						$"fileName_{i}"),
					null!));
			}

			return files;
		}

		private static List<FailedFile> CreateFailedFpFiles(int count)
		{
			List<FailedFile> files = new List<FailedFile>();
			for (int i = 0; i < count; i++)
			{
				files.Add(new FailedFile(Path.Combine(SendDir, $"failed_file_{i}"), new InvalidOperationException("failed to parse XYZ")));
			}

			return files;
		}
	}
}