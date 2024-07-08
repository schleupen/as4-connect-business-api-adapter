// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System.Globalization;
	using System.Net;
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.MP.API;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	internal sealed partial class BusinessApiGatewayTest
	{
		private sealed class Fixture : IDisposable
		{
			private const string MessageId = "EC953F86-038F-49E4-A7D9-3C417922EDB1";

			private readonly MockRepository mockRepository = new (MockBehavior.Strict);
			private readonly Mock<IJwtBuilder> jwtHelperMock;
			private readonly Mock<IClientCertificateProvider> marketpartnerCertificateProviderMock;
			private readonly Mock<ILogger<BusinessApiGateway>> loggerMock;
			private readonly Mock<IBusinessApiClientFactory> clientWrapperFactoryMock;
			private readonly Mock<IBusinessApiClient> clientWrapperMock;
			private readonly Mock<IClientCertificate> certificateMock;
			private readonly X509Certificate2 certificate;

			public Fixture()
			{
				jwtHelperMock = mockRepository.Create<IJwtBuilder>();
				marketpartnerCertificateProviderMock = mockRepository.Create<IClientCertificateProvider>();
				loggerMock = mockRepository.Create<ILogger<BusinessApiGateway>>();
				clientWrapperFactoryMock = mockRepository.Create<IBusinessApiClientFactory>();
				clientWrapperMock = mockRepository.Create<IBusinessApiClient>();
				certificateMock = mockRepository.Create<IClientCertificate>();
				certificate = new X509Certificate2(Array.Empty<byte>());
			}

			public void Dispose()
			{
				certificate.Dispose();
				mockRepository.VerifyAll();
			}

			public BusinessApiGateway CreateTestObject()
			{
				return new BusinessApiGateway(
					jwtHelperMock.Object,
					marketpartnerCertificateProviderMock.Object,
					"https://Dummy",
					"12345",
					clientWrapperFactoryMock.Object, loggerMock.Object);
			}

			public void ValidateReceiveInfo(MessageReceiveInfo receiveInfo)
			{
				Assert.That(receiveInfo.GetAvailableMessages().Length, Is.EqualTo(1));

				MpMessage message = receiveInfo.GetAvailableMessages()[0];
				Assert.That(message.BdewDocumentDate, Is.EqualTo("2024-01-15 15:55:42 +01:00"));
				Assert.That(message.CreatedAt, Is.EqualTo(new DateTimeOffset(new DateTime(2024, 01, 17, 16, 00, 00), TimeSpan.FromHours(1))));
				Assert.That(message.MessageId.ToUpper(CultureInfo.InvariantCulture), Is.EqualTo(MessageId.ToUpper(CultureInfo.InvariantCulture)));
				Assert.That(message.PartyInfo.Receiver?.Id, Is.EqualTo("ReceiverId"));
				Assert.That(message.PartyInfo.Receiver?.Type, Is.EqualTo("BDEW"));
				Assert.That(message.PartyInfo.Sender?.Id, Is.EqualTo("SenderId"));

				Assert.That(receiveInfo.ConfirmableMessages.Count,Is.EqualTo(0));
				Assert.That(receiveInfo.HasTooManyRequestsError, Is.False);
			}

			public void PrepareQueryAvailableMessages()
			{
				SetupMarketpartnerCertificateProvider();
				SetupClientWrapperFactory();
				SetupCertificate();

				clientWrapperMock
					.Setup(x => x.V1MpMessagesInboxGetAsync(It.Is<int>(limit => limit == 51), It.IsAny<CancellationToken>()))
					.Returns(Task.FromResult(new QueryInboxMessagesResponseDto
					{
						Messages = new List<InboundMPMessageDto>
						{
							new()
							{
								BdewDocType = "DocumentType",
								BdewDocumentDate = "2024-01-15 15:55:42 +01:00",
								BdewDocumentNo = "DocumentNumber",
								Created_at = new DateTimeOffset(new DateTime(2024, 01, 17, 16, 00, 00), TimeSpan.FromHours(1)),
								Error = new ErrorDto
								{
									Detail = "ErrorDetail",
									ErrorType = ErrorTypeDto.PROTOCOL,
									ResponsibleParty = ResponsiblePartyDto.AS4_CONNECT,
									Timestamp = new DateTimeOffset(new DateTime(2024, 01, 16, 00, 00, 00), TimeSpan.FromHours(1)),
									Title = "ErrorTitle"
								},
								MessageId = Guid.Parse(MessageId),
								State = InboundMessageStateDto.PROVIDED,
								PartyInfo = new PartyInfoDto
								{
									Receiver = new PartyIdentifierDto { Id = "ReceiverId", Type = PartyIdTypeDto.BDEW },
									Sender = new PartyIdentifierDto { Id = "SenderId", Type = PartyIdTypeDto.BDEW }
								},
								Payload = new PayloadDto
								{
									HashSHA256 = "Hash",
									SizeInBytes = 412
								},
								Trace = new List<InboundMessageStateDtoMessageTraceEntryDto>
								{
									new()
									{
										Message = "Message",
										State = InboundMessageStateDto.ACCEPTED,
										Timestamp = new DateTimeOffset(new DateTime(2024, 01, 16, 12, 00, 00), TimeSpan.FromHours(1)),
									}
								}
							}
						}
					}));
			}

			private void SetupClientWrapperFactory()
			{
				clientWrapperFactoryMock
					.Setup(x => x.Create(It.Is<string>(endpoint => endpoint == "https://Dummy"), It.IsAny<HttpClient>()))
					.Returns(clientWrapperMock.Object);
			}

			private void SetupMarketpartnerCertificateProvider()
			{
				marketpartnerCertificateProviderMock
					.Setup(x => x.GetCertificate(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(certificateMock.Object);
			}

			public InboxMpMessage PrepareAcknowledgeReceivedMessage()
			{
				SetupMarketpartnerCertificateProvider();
				SetupClientWrapperFactory();
				SetupCertificate();

				jwtHelperMock
					.Setup(x => x.CreateSignedToken(It.Is<InboxMpMessage>(message => message.MessageId == MessageId)))
					.Returns("SignedToken");

				clientWrapperMock
					.Setup(x => x.V1MpMessagesInboxAcknowledgementAsync(
							It.Is<Guid>(messageId => messageId == Guid.Parse(MessageId)),
							It.Is<MessageAcknowledgedRequestDto>(request => request.Jwt == "SignedToken"),
							It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);

				return CreateInboxMessage();
			}

			private InboxMpMessage CreateInboxMessage()
			{
				return new InboxMpMessage(
					MessageId,
					new DateTimeOffset(new DateTime(2024, 01, 18, 09, 28, 00), TimeSpan.FromHours(1)),
					"DocumentDate",
					new SendingParty("SendingParty", "BDEW"),
					new ReceivingParty("ReceivingParty", "BDEW"),
					string.Empty,
					null);
			}

			public InboxMpMessage PrepareAcknowledgeReceivedMessageFailed()
			{
				SetupMarketpartnerCertificateProvider();
				SetupCertificate();
				SetupClientWrapperFactory();

				jwtHelperMock
					.Setup(x => x.CreateSignedToken(It.Is<InboxMpMessage>(message => message.MessageId == MessageId)))
					.Returns("SignedToken");

				clientWrapperMock
					.Setup(x => x.V1MpMessagesInboxAcknowledgementAsync(
						It.Is<Guid>(messageId => messageId == Guid.Parse(MessageId)),
						It.Is<MessageAcknowledgedRequestDto>(request => request.Jwt == "SignedToken"),
						It.IsAny<CancellationToken>()))
					.Throws(new ApiException("something failed", (int)HttpStatusCode.Conflict, "The response", new Dictionary<string, IEnumerable<string>>(), new InvalidOperationException("The inner exception")));

				return CreateInboxMessage();
			}

			private void SetupCertificate()
			{
				certificateMock
					.Setup(x => x.AsX509Certificate())
					.Returns(certificate);
			}
		}
	}
}
