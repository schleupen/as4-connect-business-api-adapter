﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using System.Collections.ObjectModel;
	using System.Net;
	using BusinessApi;
	using Microsoft.Extensions.Logging;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	internal sealed partial class SendMessageAdapterControllerTest
	{
		private sealed class Fixture : IDisposable
		{
			private const string SenderMessageId = "53DFDBB7-9A17-4E4F-BAA9-5A787437DB71";

			private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Strict);
			private readonly Mock<IAs4BusinessApiClientFactory> businessApiClientFactory;
			private readonly Mock<IConfigurationAccess> configurationMock;
			private readonly Mock<IEdifactDirectoryResolver> edifactDirectoryResolverMock;
			private readonly Mock<ILogger<SendMessageAdapterController>> loggerMock;
			private readonly Mock<IEdifactFile> edifactFile1Mock;
			private readonly Mock<IEdifactFile> edifactFile2Mock;
			private readonly Mock<IAs4BusinessApiClient> as4BusinessApiClientMock;

			public Fixture()
			{
				businessApiClientFactory = mockRepository.Create<IAs4BusinessApiClientFactory>();
				configurationMock = mockRepository.Create<IConfigurationAccess>();
				edifactDirectoryResolverMock = mockRepository.Create<IEdifactDirectoryResolver>();
				edifactFile1Mock = mockRepository.Create<IEdifactFile>();
				edifactFile2Mock = mockRepository.Create<IEdifactFile>();
				as4BusinessApiClientMock = mockRepository.Create<IAs4BusinessApiClient>();
				loggerMock = mockRepository.Create<ILogger<SendMessageAdapterController>>(MockBehavior.Loose);
			}

			public SendMessageAdapterController CreateTestObject()
			{
				return new SendMessageAdapterController(
					businessApiClientFactory.Object,
					configurationMock.Object,
					edifactDirectoryResolverMock.Object,
					loggerMock.Object);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
			}

			public void SendAvailableMessagesAsyncWithoutSendDirectory()
			{
				SetupAdapterConfiguration();
				configurationMock
					.Setup(x => x.ReadSendDirectory())
					.Returns(string.Empty);
			}

			private void SetupAdapterConfiguration(int sendLimit = 100)
			{
				configurationMock
					.Setup(x => x.ReadAdapterConfigurationValue())
					.Returns(new AdapterConfiguration(1, sendLimit, 1, 100));
			}

			public void SendAvailableMessagesAsyncWithoutEdifactFiles()
			{
				SetupAdapterConfiguration();
				SetupSendDirectoryConfiguration();

				edifactDirectoryResolverMock
					.Setup(x => x.GetEditfactFilesFrom(It.Is<string>(path => path == "C:\\Temp")))
					.Returns(new ReadOnlyCollection<IEdifactFile>(Array.Empty<IEdifactFile>()));
			}

			public void VerifyApiWasNotCalled()
			{
				businessApiClientFactory
					.Verify(x => x.CreateAs4BusinessApiClient(It.IsAny<string>()), Times.Never);
			}

			public void SendAvailableMessagesAsync()
			{
				SetupAdapterConfiguration();
				SetupSendDirectoryConfiguration();

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
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<OutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((OutboxMessage outboxMessage) => Task.FromResult(new MessageResponse<OutboxMessage>(true, outboxMessage)));
			}

			private OutboxMessage CreateOutboxMessage()
			{
				return new OutboxMessage(new ReceivingParty("54321", "BDEW"), SenderMessageId, "DocumentNumber", "MSCONS", Array.Empty<byte>(), "test.edi", new DateTimeOffset(new DateTime(2024, 01, 23, 09, 24, 44), TimeSpan.FromHours(1)));
			}

			private void SetupSendDirectoryConfiguration()
			{
				configurationMock
					.Setup(x => x.ReadSendDirectory())
					.Returns("C:\\Temp");
			}

			public void SendAvailableMessagesAsyncWithErrorDuringSend()
			{
				SetupAdapterConfiguration();
				SetupSendDirectoryConfiguration();

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
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<OutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Throws(() => new InvalidOperationException("Expected"));
			}

			public void SendAvailableMessagesAsyncWithSenderIdentificationNumberNotResolvable()
			{
				SetupAdapterConfiguration();
				SetupSendDirectoryConfiguration();

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
				SetupSendDirectoryConfiguration();

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
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Throws(() => new InvalidOperationException("Expected during API creation"));

				businessApiClientFactory
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "54321")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<OutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((OutboxMessage outboxMessage) => Task.FromResult(new MessageResponse<OutboxMessage>(true, outboxMessage)));
			}

			public void SendAvailableMessagesAsyncWithApiExceptionForTooManyMessages()
			{
				SetupAdapterConfiguration();
				SetupSendDirectoryConfiguration();

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
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<OutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((OutboxMessage outboxMessage) => Task.FromResult(new MessageResponse<OutboxMessage>(false, outboxMessage, HttpStatusCode.TooManyRequests, new ApiException("Error from API", 429, "response", new Dictionary<string, IEnumerable<string>>(), null))));
			}

			public void VerifyTooManyMessagesErrorWasLogged()
			{
				VerifyLogger(LogLevel.Information, "Finished sending available messages: 0/2 sent successfully.A 429 TooManyRequests status code was encountered while sending the EDIFACT messages which caused the sending to end before all messages could be sent.");
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
				SetupSendDirectoryConfiguration();

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
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<OutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((OutboxMessage outboxMessage) => Task.FromResult(new MessageResponse<OutboxMessage>(false, outboxMessage, HttpStatusCode.BadGateway, new ApiException("Error from API", 502, "response", new Dictionary<string, IEnumerable<string>>(), null))));
			}

			public void SendAvailableMessagesAsyncWithMoreMessagesThanLimit()
			{
				SetupAdapterConfiguration(1);
				SetupSendDirectoryConfiguration();

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
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(as4BusinessApiClientMock.Object);

				as4BusinessApiClientMock
					.Setup(x => x.Dispose());

				as4BusinessApiClientMock
					.Setup(x => x.SendMessageAsync(It.Is<OutboxMessage>(outboxMessage => outboxMessage.SenderMessageId == SenderMessageId)))
					.Returns((OutboxMessage outboxMessage) => Task.FromResult(new MessageResponse<OutboxMessage>(true, outboxMessage)));

			}

			public void VerifySecondMessageWasNotSend()
			{
				edifactFile2Mock
					.Verify(x => x.SenderIdentificationNumber, Times.Never);
			}
		}
	}
}