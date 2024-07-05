// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
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
			private readonly Mock<IBusinessApiGatewayFactory> businessApiClientFactoryMock;
			private readonly Mock<IOptions<AdapterOptions>> adapterOptions;
			private readonly Mock<IEdifactDirectoryResolver> edifactDirectoryResolverMock;
			private readonly Mock<ILogger<ReceiveMessageAdapterController>> loggerMock;
			private readonly Mock<IOptions<ReceiveOptions>> receiveOptionsMock;
			private readonly Mock<IBusinessApiGateway> businessApiClientMock;

			public Fixture()
			{
				businessApiClientMock = mockRepository.Create<IBusinessApiGateway>();
				businessApiClientFactoryMock = mockRepository.Create<IBusinessApiGatewayFactory>();
				adapterOptions = mockRepository.Create<IOptions<AdapterOptions>>();
				edifactDirectoryResolverMock = mockRepository.Create<IEdifactDirectoryResolver>();
				loggerMock = mockRepository.Create<ILogger<ReceiveMessageAdapterController>>(MockBehavior.Loose);
				receiveOptionsMock = mockRepository.Create<IOptions<ReceiveOptions>>(MockBehavior.Loose);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
			}

			public ReceiveMessageAdapterController CreateTestObject()
			{
				return new ReceiveMessageAdapterController(
					businessApiClientFactoryMock.Object,
					edifactDirectoryResolverMock.Object,
					receiveOptionsMock.Object,
					adapterOptions.Object,
					loggerMock.Object);
			}

			public void PrepareReceiveAvailableMessagesAsync()
			{
				SetupAdapterConfiguration();
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
					new SendingParty("Sender", "BDEW"),
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
				PartyInfo partyInfo = new PartyInfo(new SendingParty("Sender", "BDEW"), new ReceivingParty("Receiver", "BDEW"));
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
				adapterOptions
					.Setup(x => x.Value)
					.Returns(new AdapterOptions() { Marketpartners = marketpartners });
			}

			private void SetupAdapterConfiguration(int messageLimit = 100)
			{
				this.receiveOptionsMock.Setup(x => x.Value).Returns(new ReceiveOptions()
				{
					MessageLimitCount = messageLimit,
					RetryCount = 0,
					Directory = @"C:\Temp"
				});
			}
		}
	}
}