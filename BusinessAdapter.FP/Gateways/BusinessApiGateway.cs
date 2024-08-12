// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using System.IO;
	using System.Text;
	using System.IO.Compression;
	using System.Net;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;
	using Schleupen.AS4.BusinessAdapter.FP.Sending;
	using Schleupen.AS4.BusinessAdapter.FP.Receiving;

	public sealed class BusinessApiGateway(
		FpParty client,
		IHttpClientFactory httpClientFactory,
		IBusinessApiClientFactory businessApiClientFactory,
		IPartyIdTypeAssembler partyIdTypeAssembler,
	    string as4BusinessApiEndpoint,
		ILogger<BusinessApiGateway> logger,
		IJwtBuilder jwtBuilder)
		: IBusinessApiGateway
	{
		private readonly HttpClient httpClient = httpClientFactory.CreateFor(client);
		private static readonly Encoding DefaultEncoding = Encoding.GetEncoding("ISO-8859-1");

		public async Task<BusinessApiResponse<FpOutboxMessage>> SendMessageAsync(
			FpOutboxMessage message, 
			CancellationToken cancellationToken)
		{
			logger.LogInformation("Sending {FileName} from {Sender} to {Receiver} [MessageId:'{}']",
				message.FileName,
				message.Sender,
				message.Receiver,
				message.MessageId);

			using (MemoryStream compressedStream = new MemoryStream())
			{
				using (MemoryStream payloadStream = new MemoryStream(message.Payload))
				using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
				{
					await payloadStream.CopyToAsync(gZipStream);
				}

				compressedStream.Position = 0;
				try
				{
					var businessApiClient = businessApiClientFactory.Create(httpClient.BaseAddress!, httpClient);
					await businessApiClient.V1FpMessagesOutboxPostAsync(message.Receiver.Id,
						partyIdTypeAssembler.ToPartyTypeDto(message.Receiver.Type),
						new FileParameter(compressedStream, message.FileName),
						message.BDEWProperties.BDEWDocumentType,
						message.BDEWProperties.BDEWDocumentNo,
						message.BDEWProperties.BDEWFulfillmentDate,
						message.BDEWProperties.BDEWSubjectPartyId,
						message.BDEWProperties.BDEWSubjectPartyRole,
						message.MessageId,
						message.SenderMessageId,
						cancellationToken);

					return new BusinessApiResponse<FpOutboxMessage>(true, message);
				}
				catch (ApiException ex)
				{
					return new BusinessApiResponse<FpOutboxMessage>(false, message, (HttpStatusCode)ex.StatusCode, ex);
				}
			}
		}

		public async Task<MessageReceiveInfo> QueryAvailableMessagesAsync(int limit = 50)
		{
			IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
			QueryInboxFPMessagesResponseDto clientResponse = await businessApiClient.V1FpMessagesInboxGetAsync(limit);

			List<As4FpMessage> messages = new List<As4FpMessage>();
			
			foreach (InboundFPMessageDto? message in clientResponse.Messages)
			{
				try
				{
					if (message.PartyInfo == null)
					{
						throw new ArgumentException($"The message with the identification {message.MessageId} has no party info.");
					}

					messages.Add(new As4FpMessage(
						message.Created_at,
						message.MessageId.ToString(),
						new PartyInfo(
							new SendingParty(
								message.PartyInfo.Sender.Id,
								message.PartyInfo.Sender.Type.ToString()),
							new ReceivingParty(
								message.PartyInfo.Receiver.Id, 
								message.PartyInfo.Receiver.Type.ToString())),
						message.BdewDocumentNo!,
						message.BdewFulfillmentDate!,
						message.BdewSubjectPartyId,
						message.BdewSubjectPartyRole!,
						message.BdewDocumentType!));
				}
				catch (Exception e)
				{
					logger.LogError(new EventId(0), e, "Error while querying available messages");
				}
			}

			return new MessageReceiveInfo(messages.ToArray());
		}

		public async Task<BusinessApiResponse<InboxFpMessage>> ReceiveMessageAsync(As4FpMessage mpMessage)
		{
			IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
			try
			{
				FileResponse clientResponse = await businessApiClient.V1FpMessagesInboxPayloadAsync(Guid.Parse(mpMessage.MessageId));

				using (MemoryStream ms = new MemoryStream())
				{
					await clientResponse.Stream.CopyToAsync(ms);
					byte[] zippedContent = ms.ToArray();
					ms.Position = 0;

					using (GZipStream gZipStream = new GZipStream(ms, CompressionMode.Decompress, true))
					{
						using (StreamReader decompressedReader = new StreamReader(gZipStream, DefaultEncoding))
						{
							string xmlString = await decompressedReader.ReadToEndAsync();

							return new BusinessApiResponse<InboxFpMessage>(
								true,
								new InboxFpMessage(
									mpMessage.MessageId,
									mpMessage.PartyInfo.Sender!,
									mpMessage.PartyInfo.Receiver!,
									xmlString,
									zippedContent,
									new FpBDEWProperties(mpMessage.BDEWDocumentType,
										mpMessage.BdewDocumentNo,
										mpMessage.BdewFulfillmentDate,
										mpMessage.BdewSubjectPartyId,
										mpMessage.BdewSubjectPartyRole)));
						}
					}
				}
			}
			catch (ApiException ex)
			{
				return new BusinessApiResponse<InboxFpMessage>(false,
					new InboxFpMessage(mpMessage.MessageId,
						mpMessage.PartyInfo.Sender!,
						mpMessage.PartyInfo.Receiver!,
						null,
						null,
						null)
					,
					(HttpStatusCode)ex.StatusCode,
					ex);
			}
		}
		
		public async Task<BusinessApiResponse<bool>> AcknowledgeReceivedMessageAsync(InboxFpMessage fpMessage)
		{
			string tokenString = jwtBuilder.CreateSignedToken(fpMessage);
			IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
			try
			{
				if (fpMessage.MessageId == null)
				{
					throw new InvalidOperationException("The message does not have a MessageId.");
				}

				await businessApiClient.V1FpMessagesInboxAcknowledgementAsync(Guid.Parse(fpMessage.MessageId), new MessageAcknowledgedRequestDto { Jwt = tokenString });

				return new BusinessApiResponse<bool>(true, true);
			}
			catch (ApiException ex)
			{
				return new BusinessApiResponse<bool>(false, false, (HttpStatusCode)ex.StatusCode, ex);
			}
		}
		
		public void Dispose()
		{
			httpClient.Dispose();
		}
	}
}