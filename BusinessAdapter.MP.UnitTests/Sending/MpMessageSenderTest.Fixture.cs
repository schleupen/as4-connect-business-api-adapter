// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using System.Collections.ObjectModel;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.MP;
	using Schleupen.AS4.BusinessAdapter.MP.API;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	internal sealed partial class MpMessageSenderTest
	{
		private sealed class Fixture : IDisposable
		{
			private const string SenderMessageId = "53DFDBB7-9A17-4E4F-BAA9-5A787437DB71";

			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IBusinessApiGatewayFactory> businessApiClientFactory;
			private readonly Mock<IEdifactDirectoryResolver> edifactDirectoryResolverMock;
			private readonly Mock<ILogger<MpMessageSender>> loggerMock;
			private readonly Mock<IEdifactFile> edifactFile1Mock;
			private readonly Mock<IEdifactFile> edifactFile2Mock;
			private readonly Mock<IBusinessApiGateway> as4BusinessApiClientMock;
			private readonly Mock<IOptions<SendOptions>> sendOptionsMock;

			public Fixture()
			{
				businessApiClientFactory = mockRepository.Create<IBusinessApiGatewayFactory>();
				edifactDirectoryResolverMock = mockRepository.Create<IEdifactDirectoryResolver>();
				edifactFile1Mock = mockRepository.Create<IEdifactFile>();
				edifactFile2Mock = mockRepository.Create<IEdifactFile>();
				as4BusinessApiClientMock = mockRepository.Create<IBusinessApiGateway>();
				loggerMock = mockRepository.Create<ILogger<MpMessageSender>>(MockBehavior.Loose);
				sendOptionsMock = mockRepository.Create<IOptions<SendOptions>>(MockBehavior.Loose);
			}

			public MpMessageSender CreateTestObject()
			{
				return new MpMessageSender(
					businessApiClientFactory.Object,
					edifactDirectoryResolverMock.Object,
					sendOptionsMock.Object,
					loggerMock.Object);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
			}

			public void SendAvailableMessagesAsyncWithoutSendDirectory()
			{
				this.sendOptionsMock
					.Setup(x => x.Value)
					.Returns(new SendOptions() { Directory = null! });
			}

			private void SetupAdapterConfiguration(int sendLimit = 100)
			{
				this.sendOptionsMock.Setup(x => x.Value).Returns(new SendOptions()
				{
					Retry = new RetryOption() { Count = 3 },
					Directory = @"C:\Temp",
					MessageLimitCount = sendLimit
				});
			}

			public void SendAvailableMessagesAsyncWithoutEdifactFiles()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(Array.Empty<IEdifactFile>()));
			}

			public void VerifyApiWasNotCalled()
			{
				businessApiClientFactory
					.Verify(x => x.CreateGateway(It.IsAny<string>()), Times.Never);
			}

			public void SendAvailableMessagesAsync()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object
					}));

				edifactDirectoryResolverMock
					.Setup(x => x.DeleteFile(It.Is<string>(path => path == @"C:\Temp\test.edi")));

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.CreateOutboxMessage())
					.Returns(CreateOutboxMessage);

				edifactFile1Mock
					.Setup(x => x.Path)
					.Returns(@"C:\Temp\test.edi");

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<MpOutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((MpOutboxMessage outboxMessage) => Task.FromResult(new BusinessApiResponse<MpOutboxMessage>(true, outboxMessage)));
			}

			private MpOutboxMessage CreateOutboxMessage()
			{
				return new MpOutboxMessage(new ReceivingParty("54321", "BDEW"), SenderMessageId, "DocumentNumber", "MSCONS", Array.Empty<byte>(), "test.edi",
					new DateTimeOffset(new DateTime(2024, 01, 23, 09, 24, 44), TimeSpan.FromHours(1)));
			}

			public void SendAvailableMessagesAsyncWithErrorDuringSend()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object
					}));

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.CreateOutboxMessage())
					.Returns(CreateOutboxMessage);

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<MpOutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Throws(() => new InvalidOperationException("Expected"));
			}

			public void SendAvailableMessagesAsyncWithSenderIdentificationNumberNotResolvable()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object
					}));
				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns((string?)null);

				edifactFile1Mock
					.Setup(x => x.Path)
					.Returns(@"C:\Temp\test.edi");
			}

			public void SendAvailableMessagesAsyncWithExceptionDuringApiCreation()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object,
						edifactFile2Mock.Object
					}));

				edifactDirectoryResolverMock
					.Setup(x => x.DeleteFile(It.Is<string>(path => path == @"C:\Temp\test2.edi")));

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile2Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("54321");

				edifactFile2Mock
					.Setup(x => x.CreateOutboxMessage())
					.Returns(CreateOutboxMessage);

				edifactFile2Mock
					.Setup(x => x.Path)
					.Returns(@"C:\Temp\test2.edi");

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Throws(() => new InvalidOperationException("Expected during API creation"));

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "54321")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<MpOutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((MpOutboxMessage outboxMessage) => Task.FromResult(new BusinessApiResponse<MpOutboxMessage>(true, outboxMessage)));
			}

			public void SendAvailableMessagesAsyncWithApiExceptionForTooManyMessages()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object,
						edifactFile2Mock.Object
					}));

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.CreateOutboxMessage())
					.Returns(CreateOutboxMessage);

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<MpOutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((MpOutboxMessage outboxMessage) => Task.FromResult(new BusinessApiResponse<MpOutboxMessage>(false, outboxMessage,
						HttpStatusCode.TooManyRequests,
						new ApiException("Error from API", 429, "response", new Dictionary<string, IEnumerable<string>>(), null))));
			}

			public void VerifyTooManyMessagesErrorWasLogged()
			{
				VerifyLogger(LogLevel.Information,
					"Finished sending available messages: 0/2 sent successfully.A 429 TooManyRequests status code was encountered while sending the EDIFACT messages which caused the sending to end before all messages could be sent.");
			}

			private void VerifyLogger(LogLevel expectedLogLevel, string expectedMessage)
			{
				Func<object, Type, bool> state = (v, _) => string.Equals(v.ToString(), expectedMessage, StringComparison.OrdinalIgnoreCase);

				loggerMock.Verify(
					x => x.Log(
						It.Is<LogLevel>(l => l == expectedLogLevel),
						It.IsAny<EventId>(),
						It.Is<It.IsAnyType>((v, t) => state(v, t)),
						It.IsAny<Exception>(),
						It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
			}

			public void SendAvailableMessagesAsyncWithApiException()
			{
				SetupAdapterConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object,
						edifactFile2Mock.Object
					}));

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.CreateOutboxMessage())
					.Returns(CreateOutboxMessage);

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<MpOutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((MpOutboxMessage outboxMessage) => Task.FromResult(new BusinessApiResponse<MpOutboxMessage>(false, outboxMessage,
						HttpStatusCode.BadGateway, new ApiException("Error from API", 502, "response", new Dictionary<string, IEnumerable<string>>(), null))));
			}

			public void SendAvailableMessagesAsyncWithMoreMessagesThanLimit()
			{
				SetupAdapterConfiguration(1);

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(new List<IEdifactFile>
					{
						edifactFile1Mock.Object,
						edifactFile2Mock.Object
					}));

				edifactDirectoryResolverMock
					.Setup(x => x.DeleteFile(It.Is<string>(path => path == @"C:\Temp\test.edi")));

				edifactFile1Mock
					.Setup(x => x.SenderIdentificationNumber)
					.Returns("12345");

				edifactFile1Mock
					.Setup(x => x.CreateOutboxMessage())
					.Returns(CreateOutboxMessage);

				edifactFile1Mock
					.Setup(x => x.Path)
					.Returns(@"C:\Temp\test.edi");

				businessApiClientFactory
					.Setup(x => x.CreateGateway(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<MpOutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((MpOutboxMessage outboxMessage) => Task.FromResult(new BusinessApiResponse<MpOutboxMessage>(true, outboxMessage)));
			}

			public void VerifySecondMessageWasNotSend()
			{
				edifactFile2Mock
					.Verify(x => x.SenderIdentificationNumber, Times.Never);
			}
		}
	}
}