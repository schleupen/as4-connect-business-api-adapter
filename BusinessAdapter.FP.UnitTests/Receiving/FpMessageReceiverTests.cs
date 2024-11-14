using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Gateways;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.API;

namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Receiving
{
	[TestFixture]
	public class FpMessageReceiverTests
	{
		private Mock<ILogger<FpMessageReceiver>> loggerMock = null!;
		private IOptions<ReceiveOptions> receiveOptionsMock = null!;
		private IOptions<AdapterOptions> adapterOptionsMock = null!;
		private Mock<IBusinessApiGatewayFactory> businessApiGatewayFactoryMock = null!;
		private Mock<IFpFileRepository> fpFileRepositoryMock = null!;
		private IOptions<EICMapping> eicMappingMock = null!;
		private FpMessageReceiver fpMessageReceiver = null!;

		[SetUp]
		public void SetUp()
		{
			loggerMock = new Mock<ILogger<FpMessageReceiver>>();
			receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions());
			adapterOptionsMock = Options.Create<AdapterOptions>(new AdapterOptions());
			businessApiGatewayFactoryMock = new Mock<IBusinessApiGatewayFactory>();
			fpFileRepositoryMock = new Mock<IFpFileRepository>();
			eicMappingMock = Options.Create<EICMapping>(new EICMapping());

			adapterOptionsMock.Value.Marketpartners = new[] { "9912345000002" };

			List<EICMappingEntry> mappedPartyMock = new List<EICMappingEntry>()
			{
				new EICMappingEntry()
				{
					Bilanzkreis = "FINGRID",
					EIC = "asga",
					FahrplanHaendlerTyp = "PPS",
					MarktpartnerTyp = "BDEW"
				}
			};
			eicMappingMock.Value.Add("9912345000002", mappedPartyMock);

			fpMessageReceiver = new FpMessageReceiver(
				loggerMock.Object,
				receiveOptionsMock,
				adapterOptionsMock,
				businessApiGatewayFactoryMock.Object,
				fpFileRepositoryMock.Object,
				eicMappingMock);
		}

		[Test]
		public async Task ReceiveAvailableMessagesAsync_ReceiveStatusIsCorrect_ForSuccesfulMessage()
		{
			// Arrange
			receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
			{
				Directory = "C:\\Adapter\\Receive",
				MessageLimitCount = 10,
				Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
			});
			fpMessageReceiver = new FpMessageReceiver(
				loggerMock.Object,
				receiveOptionsMock,
				adapterOptionsMock,
				businessApiGatewayFactoryMock.Object,
				fpFileRepositoryMock.Object,
				eicMappingMock);
			var gatewayMock = new Mock<IBusinessApiGateway>();
			var messages = new[] { CreateFpInboxMessage() };
			var receiveInfo = new MessageReceiveInfo(messages);
			var businessApiResponse = new BusinessApiResponse<InboxFpMessage>(true, CreateDummyFpMessage());

			businessApiGatewayFactoryMock
				.Setup(factory => factory.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);
			gatewayMock
				.Setup(g => g.QueryAvailableMessagesAsync(It.IsAny<int>()))
				.ReturnsAsync(receiveInfo);
			gatewayMock
				.Setup(x => x.ReceiveMessageAsync(It.IsAny<FpInboxMessage>()))
				.ReturnsAsync(businessApiResponse);
			gatewayMock
				.Setup(x => x.AcknowledgeReceivedMessageAsync(It.IsAny<InboxFpMessage>()))
				.ReturnsAsync(new BusinessApiResponse<bool>(true, true));
			// Act
			var test = await fpMessageReceiver.ReceiveMessagesAsync(CancellationToken.None);
			Assert.That(test, Is.Not.Null);
			Assert.That(test.SuccessfulMessages.Count, Is.EqualTo(1));
			Assert.That(test.FailedMessages.Count, Is.EqualTo(0));
			Assert.That(test.TotalMessageCount, Is.EqualTo(1));
		}

		[Test]
		public void ReceiveAvailableMessagesAsync_ReceiveStatusIsCorrect_ForFailedAcknowledge()
		{
			// Arrange
			receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
			{
				Directory = "C:\\Adapter\\Receive",
				MessageLimitCount = 10,
				Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
			});
			fpMessageReceiver = new FpMessageReceiver(
				loggerMock.Object,
				receiveOptionsMock,
				adapterOptionsMock,
				businessApiGatewayFactoryMock.Object,
				fpFileRepositoryMock.Object,
				eicMappingMock);
			var gatewayMock = new Mock<IBusinessApiGateway>();
			var messages = new[] { CreateFpInboxMessage() };
			var receiveInfo = new MessageReceiveInfo(messages);
			var businessApiResponse = new BusinessApiResponse<InboxFpMessage>(true, CreateDummyFpMessage());

			businessApiGatewayFactoryMock
				.Setup(factory => factory.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);
			gatewayMock
				.Setup(g => g.QueryAvailableMessagesAsync(It.IsAny<int>()))
				.ReturnsAsync(receiveInfo);
			gatewayMock
				.Setup(x => x.ReceiveMessageAsync(It.IsAny<FpInboxMessage>()))
				.ReturnsAsync(businessApiResponse);
			gatewayMock
				.Setup(x => x.AcknowledgeReceivedMessageAsync(It.IsAny<InboxFpMessage>()))
				.ReturnsAsync(new BusinessApiResponse<bool>(false, false));
			// Act
			Assert.That(async () => await fpMessageReceiver.ReceiveMessagesAsync(CancellationToken.None),
				Throws.TypeOf<AggregateException>());
		}

		[Test]
		public void ReceiveAvailableMessagesAsync_ThrowsCatastrophicException_WhenReceiveDirectoryIsNotConfigured()
		{
			receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
			{
				Directory = string.Empty,
				MessageLimitCount = 10,
				Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
			});
			// Act & Assert
			Assert.That(async () => await fpMessageReceiver.ReceiveMessagesAsync(CancellationToken.None),
				Throws.TypeOf<CatastrophicException>()
					.With.Message.EqualTo("The receive directory is not configured."));
		}

		[Test]
		public void ReceiveAvailableMessagesAsync_ThrowsCatastrophicException_WhenNoMarketPartnersAreConfigured()
		{
			adapterOptionsMock.Value.Marketpartners = [];
			receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
			{
				Directory = "C:\\Adapter\\Receive",
				MessageLimitCount = 10,
				Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
			});
			fpMessageReceiver = new FpMessageReceiver(
				loggerMock.Object,
				receiveOptionsMock,
				adapterOptionsMock,
				businessApiGatewayFactoryMock.Object,
				fpFileRepositoryMock.Object,
				eicMappingMock);

			// Act & Assert
			Assert.That(async () => await fpMessageReceiver.ReceiveMessagesAsync(CancellationToken.None),
				Throws.TypeOf<CatastrophicException>()
					.With.Message.EqualTo("No valid own market partners were found."));
		}

		[Test]
		public void ReceiveAvailableMessagesAsync_ShouldTrackMarketPartnersWithoutCertificate()
		{
			// Arrange
			receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
			{
				Directory = "C:\\Adapter\\Receive",
				MessageLimitCount = 10,
				Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
			});
			fpMessageReceiver = new FpMessageReceiver(
				loggerMock.Object,
				receiveOptionsMock,
				adapterOptionsMock,
				businessApiGatewayFactoryMock.Object,
				fpFileRepositoryMock.Object,
				eicMappingMock);
			var gatewayMock = new Mock<IBusinessApiGateway>();
			var messages = new[] { CreateFpInboxMessage() };
			var receiveInfo = new MessageReceiveInfo(messages);

			businessApiGatewayFactoryMock
				.Setup(factory => factory.CreateGateway(It.IsAny<FpParty>()))
				.Returns(gatewayMock.Object);

			gatewayMock
				.Setup(g => g.QueryAvailableMessagesAsync(It.IsAny<int>()))
				.ThrowsAsync(new Schleupen.AS4.BusinessAdapter.Certificates.MissingCertificateException("TestMarketPartner"));

			// Act
			var ex = Assert.ThrowsAsync<AggregateException>(async () =>
				await fpMessageReceiver.ReceiveMessagesAsync(CancellationToken.None));

			// Assert
			var innerException = ex.InnerExceptions[0];
			Assert.That(innerException, Is.InstanceOf<Schleupen.AS4.BusinessAdapter.Certificates.MissingCertificateException>());
			Assert.That(((Schleupen.AS4.BusinessAdapter.Certificates.MissingCertificateException)innerException).MarketpartnerIdentificationNumber,
				Is.EqualTo("TestMarketPartner"));
		}

		public FpInboxMessage CreateFpInboxMessage(
			DateTimeOffset? createdAt = null,
			string messageId = "TestMessageId",
			PartyInfo? partyInfo = null,
			string bdewDocumentNo = "12345",
			string bdewFulfillmentDate = "2024-08-12",
			string bdewSubjectPartyId = "PartyId123",
			string bdewSubjectPartyRole = "Sender",
			string bdewDocumentType = "Type1")
		{
			return new FpInboxMessage(
				createdAt ?? DateTimeOffset.UtcNow,
				messageId,
				partyInfo ?? CreateDefaultPartyInfo(),
				bdewDocumentNo,
				bdewFulfillmentDate,
				bdewSubjectPartyId,
				bdewSubjectPartyRole,
				bdewDocumentType);
		}

		public InboxFpMessage CreateDummyFpMessage()
		{
			return new InboxFpMessage(
				"TestMessageId",
				new SendingParty("DefaultSendingPartyId", "DefaultSendingPartyRole"),
				new ReceivingParty("DefaultReceivingPartyId", "DefaultReceivingPartyRole"),
				CreateDummyMessageContent().ToString(),
				CreateDummyMessageContent().ToArray(),
				new FpBDEWProperties(
					"docType",
					"docNo",
					"docDate",
					"docSubjectId",
					"docSubjectRole")
			);
		}

		private PartyInfo CreateDefaultPartyInfo()
		{
			return new PartyInfo(
				sender: new SendingParty("DefaultSendingPartyId", "DefaultSendingPartyRole"),
				receiver: new ReceivingParty("DefaultReceivingPartyId", "DefaultReceivingPartyRole")
			);
		}

		private MemoryStream CreateDummyMessageContent()
		{
			string dirOfTestFile = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Parsing/20240125_PPS_FINGRID_0X1001A1001A264_002.xml");
			MemoryStream ms = new MemoryStream();
			using (FileStream file = new FileStream(dirOfTestFile, FileMode.Open, FileAccess.Read))
				file.CopyTo(ms);
			return ms;
		}
	}
}