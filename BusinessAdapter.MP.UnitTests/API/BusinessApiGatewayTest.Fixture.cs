// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using System.Net;
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	internal sealed partial class BusinessApiGatewayTest
	{
		internal class TestData
		{
			public readonly InboxMpMessage InboxMpMessage;

			public TestData()
			{
				this.InboxMpMessage = CreateInboxMessage();
			}

			private InboxMpMessage CreateInboxMessage()
			{
				return new InboxMpMessage(
					Guid.NewGuid().ToString(),
					new DateTimeOffset(new DateTime(2024, 01, 18, 09, 28, 00), TimeSpan.FromHours(1)),
					"DocumentDate",
					new SendingParty("SendingParty", "BDEW"),
					new ReceivingParty("ReceivingParty", "BDEW"),
					string.Empty,
					null);
			}
		}

		private sealed class Fixture : IDisposable
		{
			public TestData Data { get; } = new();
			private readonly MockRepository mockRepository = new (MockBehavior.Strict);
			private readonly Mock<IJwtBuilder> jwtHelperMock;
			private readonly Mock<ILogger<BusinessApiGateway>> loggerMock;
			private readonly Mock<IBusinessApiClientFactory> businessApiClientFactoryMock;
			private readonly Mock<IBusinessApiClient> businessApiClientMock;
			private readonly Mock<IPartyIdTypeAssembler> partyIdTypeAssembler;
			private readonly X509Certificate2 certificate;
			private readonly Mock<IHttpClientFactory> httpClientFactoryMock;


			public Fixture()
			{
				jwtHelperMock = mockRepository.Create<IJwtBuilder>();
				loggerMock = mockRepository.Create<ILogger<BusinessApiGateway>>();
				businessApiClientFactoryMock = mockRepository.Create<IBusinessApiClientFactory>();
				businessApiClientMock = mockRepository.Create<IBusinessApiClient>();
				certificate = new X509Certificate2(Array.Empty<byte>());
				partyIdTypeAssembler = mockRepository.Create<IPartyIdTypeAssembler>(MockBehavior.Loose);
				httpClientFactoryMock = mockRepository.Create<IHttpClientFactory>(MockBehavior.Loose);
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
					"https://Dummy",
					"12345",
					businessApiClientFactoryMock.Object,
					partyIdTypeAssembler.Object,
					httpClientFactoryMock.Object,
					loggerMock.Object);
			}

			public void PrepareQueryAvailableMessages()
			{
				SetupBusinessApiClientFactoryMock();
				SetupHttpClientFactoryMock();

				businessApiClientMock
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
								MessageId = Guid.Parse(Data.InboxMpMessage.MessageId),
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

			private void SetupHttpClientFactoryMock()
			{
				this.httpClientFactoryMock.Setup(x => x.CreateFor(It.Is<Party>(x => x.Id == "12345"))).Returns(
#pragma warning disable CA2000
					new HttpClient());
#pragma warning restore CA2000
			}

			private void SetupBusinessApiClientFactoryMock()
			{
				businessApiClientFactoryMock
					.Setup(x => x.Create(It.Is<Uri>(endpoint => endpoint.AbsoluteUri == "https://dummy/"), It.IsAny<HttpClient>()))
					.Returns(businessApiClientMock.Object);
			}

			public InboxMpMessage PrepareAcknowledgeReceivedMessage()
			{
				SetupBusinessApiClientFactoryMock();
				SetupHttpClientFactoryMock();

				jwtHelperMock
					.Setup(x => x.CreateSignedToken(It.Is<InboxMpMessage>(message => message.MessageId == Data.InboxMpMessage.MessageId)))
					.Returns("SignedToken");

				businessApiClientMock
					.Setup(x => x.V1MpMessagesInboxAcknowledgementAsync(
							It.Is<Guid>(messageId => messageId == Guid.Parse(Data.InboxMpMessage.MessageId)),
							It.Is<MessageAcknowledgedRequestDto>(request => request.Jwt == "SignedToken"),
							It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);

				return Data.InboxMpMessage;
			}



			public InboxMpMessage PrepareAcknowledgeReceivedMessageFailed()
			{
				SetupBusinessApiClientFactoryMock();
				SetupHttpClientFactoryMock();

				jwtHelperMock
					.Setup(x => x.CreateSignedToken(It.Is<InboxMpMessage>(message => message.MessageId == Data.InboxMpMessage.MessageId)))
					.Returns("SignedToken");

				businessApiClientMock
					.Setup(x => x.V1MpMessagesInboxAcknowledgementAsync(
						It.Is<Guid>(messageId => messageId == Guid.Parse(Data.InboxMpMessage.MessageId)),
						It.Is<MessageAcknowledgedRequestDto>(request => request.Jwt == "SignedToken"),
						It.IsAny<CancellationToken>()))
					.Throws(new ApiException("something failed", (int)HttpStatusCode.Conflict, "The response", new Dictionary<string, IEnumerable<string>>(), new InvalidOperationException("The inner exception")));

				return Data.InboxMpMessage;
			}
		}
	}
}
