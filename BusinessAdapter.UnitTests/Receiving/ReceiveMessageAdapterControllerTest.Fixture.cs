// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using Microsoft.Extensions.Logging;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.MP;
	using Schleupen.AS4.BusinessAdapter.MP.API;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	internal sealed partial class ReceiveMessageAdapterControllerTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IAs4BusinessApiClientFactory> businessApiClientFactoryMock;
			private readonly Mock<IConfigurationAccess> configurationAccessMock;
			private readonly Mock<IEdifactDirectoryResolver> edifactDirectoryResolverMock;
			private readonly Mock<ILogger<ReceiveMessageAdapterController>> loggerMock;
			private readonly Mock<IAs4BusinessApiClient> businessApiClientMock;

			public Fixture()
			{
				businessApiClientMock = mockRepository.Create<IAs4BusinessApiClient>();
				businessApiClientFactoryMock = mockRepository.Create<IAs4BusinessApiClientFactory>();
				configurationAccessMock = mockRepository.Create<IConfigurationAccess>();
				edifactDirectoryResolverMock = mockRepository.Create<IEdifactDirectoryResolver>();
				loggerMock = mockRepository.Create<ILogger<ReceiveMessageAdapterController>>(MockBehavior.Loose);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
			}

			public ReceiveMessageAdapterController CreateTestObject()
			{
				return new ReceiveMessageAdapterController(
					businessApiClientFactoryMock.Object,
					configurationAccessMock.Object,
					edifactDirectoryResolverMock.Object,
					loggerMock.Object);
			}

			public void PrepareReceiveAvailableMessagesAsync()
			{
				SetupAdapterConfiguration();
				SetupReadReceiveDirectory();
				SetupOwnMarketpartners("12345");
				SetupBusinessApiClientFactory("12345");

				businessApiClientMock
					.Setup(x => x.QueryAvailableMessagesAsync(It.Is<int>(limit => limit == 100)))
					.Returns(CreateMessageReceiveInfo());

				businessApiClientMock
					.Setup(x => x.ReceiveMessageAsync(It.Is<MpMessage>(message => message.MessageId == "1")))
					.Returns(Task.FromResult(new MessageResponse<InboxMpMessage>(true, CreateInboxMessage("1"))));

				businessApiClientMock
					.Setup(x => x.ReceiveMessageAsync(It.Is<MpMessage>(message => message.MessageId == "2")))
					.Returns(Task.FromResult(new MessageResponse<InboxMpMessage>(true, CreateInboxMessage("2"))));

				businessApiClientMock
					.Setup(x => x.Dispose());

				edifactDirectoryResolverMock
					.Setup(x => x.StoreEdifactFileTo(It.Is<InboxMpMessage>(message => message.MessageId == "1"), @"C:\Temp"))
					.Returns(@"C:\Temp\test1.edi");

				edifactDirectoryResolverMock
					.Setup(x => x.StoreEdifactFileTo(It.Is<InboxMpMessage>(message => message.MessageId == "2"), @"C:\Temp"))
					.Returns(@"C:\Temp\test2.edi");

				businessApiClientMock
					.Setup(x => x.AcknowledgeReceivedMessageAsync(It.Is<InboxMpMessage>(message => message.MessageId == "1")))
					.Returns(Task.FromResult(new MessageResponse<bool>(true, true)));

				businessApiClientMock
					.Setup(x => x.AcknowledgeReceivedMessageAsync(It.Is<InboxMpMessage>(message => message.MessageId == "2")))
					.Returns(Task.FromResult(new MessageResponse<bool>(true, true)));
			}

			private InboxMpMessage CreateInboxMessage(string messageId)
			{
				return new InboxMpMessage(
					messageId,
					DateTimeOffset.Now,
					"DocumentDate",
					new SendingParty("Sender"),
					new ReceivingParty("Receiver", "BDew"),
					"UNA",
					Array.Empty<byte>());
			}

			private Task<MessageReceiveInfo> CreateMessageReceiveInfo()
			{
				return Task.FromResult(new MessageReceiveInfo(
				[
					CreateAvailableMessage("1"),
					CreateAvailableMessage("2")
				]));
			}

			private MpMessage CreateAvailableMessage(string messageId)
			{
				PartyInfo partyInfo = new PartyInfo(new SendingParty("Sender"), new ReceivingParty("Receiver", "BDEW"));
				return new MpMessage(DateTimeOffset.Now, $"DocumentDate{messageId}", messageId, partyInfo);
			}

			private void SetupBusinessApiClientFactory(string expectedMarketpartnerId)
			{
				businessApiClientFactoryMock
					.Setup(x => x.CreateAs4BusinessApiClient(It.Is<string>(marketpartnerId => marketpartnerId == expectedMarketpartnerId)))
					.Returns(businessApiClientMock.Object);
			}

			private void SetupOwnMarketpartners(params string[] marketpartners)
			{
				configurationAccessMock
					.Setup(x => x.ReadOwnMarketpartners())
					.Returns(marketpartners);
			}

			private void SetupReadReceiveDirectory()
			{
				configurationAccessMock
					.Setup(x => x.ReadReceiveDirectory())
					.Returns(@"C:\Temp");
			}

			private void SetupAdapterConfiguration(int messageLimit = 100)
			{
				configurationAccessMock
					.Setup(x => x.ReadAdapterConfigurationValue())
					.Returns(new AdapterConfiguration(0, 0, 1, messageLimit));
			}
		}
	}
}
