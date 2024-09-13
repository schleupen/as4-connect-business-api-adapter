// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Net;
	using System.Net.Http;
	using System.Security.Authentication;
	using System.Text;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;
	using HttpStatusCode = Schleupen.AS4.BusinessAdapter.HttpStatusCode;

	public sealed class BusinessApiGateway : IBusinessApiGateway
	{
		private static readonly Encoding DefaultEncoding = Encoding.GetEncoding("ISO-8859-1");

		private readonly IJwtBuilder jwtBuilder;
		private readonly string as4BusinessApiEndpoint;
		private readonly ILogger<BusinessApiGateway> logger;
		private readonly IBusinessApiClientFactory businessApiClientFactory;
		private readonly HttpClient httpClient;
		private readonly HttpClientHandler httpClientHandler;
		private readonly IPartyIdTypeAssembler partyIdTypeAssembler;

		public BusinessApiGateway(IJwtBuilder jwtBuilder,
			IClientCertificateProvider clientCertificateProvider,
			string as4BusinessApiEndpoint,
			string marketpartnerIdentification,
			IBusinessApiClientFactory businessApiClientFactory,
			IPartyIdTypeAssembler partyIdTypeAssembler,
			ILogger<BusinessApiGateway> logger)
		{
			this.jwtBuilder = jwtBuilder;
			this.as4BusinessApiEndpoint = as4BusinessApiEndpoint;
			this.logger = logger;
			this.partyIdTypeAssembler = partyIdTypeAssembler;
			this.businessApiClientFactory = businessApiClientFactory;

			IClientCertificate resolvedMarketpartnerCertificate = clientCertificateProvider.GetCertificate(marketpartnerIdentification);
			httpClientHandler = InitializeHttpClientHandler(resolvedMarketpartnerCertificate);
			httpClient = InitializeHttpClient();
		}

		public async Task<BusinessApiResponse<MpOutboxMessage>> SendMessageAsync(MpOutboxMessage message)
		{
			using (MemoryStream compressedStream = new MemoryStream())
			{
				using (MemoryStream payloadStream = new MemoryStream(message.Payload))
				using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
				{
					await payloadStream.CopyToAsync(gZipStream);
				}

				compressedStream.Position = 0;
				IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
				try
				{
					await businessApiClient.V1MpMessagesOutboxPostAsync(message.Receiver.Id,
						partyIdTypeAssembler.ToPartyTypeDto(message.Receiver.Type),
						new FileParameter(compressedStream, message.FileName),
						message.BdewDocumentType,
						message.BdewDocumentNumber,
						message.DocumentDate.ToString("yyyy-MM-dd"),
						Guid.NewGuid(),
						message.SenderMessageId);

					return new BusinessApiResponse<MpOutboxMessage>(true, message);
				}
				catch (ApiException ex)
				{
					return new BusinessApiResponse<MpOutboxMessage>(false, message, (HttpStatusCode)ex.StatusCode, ex);
				}
			}
		}

		public async Task<MessageReceiveInfo> QueryAvailableMessagesAsync(int limit = 50)
		{
			IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
			QueryInboxMessagesResponseDto clientResponse = await businessApiClient.V1MpMessagesInboxGetAsync(limit);

			List<MpMessage> messages = new List<MpMessage>();

			foreach (InboundMPMessageDto? message in clientResponse.Messages)
			{
				try
				{
					if (message.PartyInfo == null)
					{
						throw new ArgumentException($"The message with the identification {message.MessageId} has no party info.");
					}

					messages.Add(new MpMessage(
						message.Created_at,
						message.BdewDocumentDate,
						message.MessageId.ToString(),
						new PartyInfo(new SendingParty(message.PartyInfo.Sender.Id, message.PartyInfo.Sender.Type.ToString()), new ReceivingParty(message.PartyInfo.Receiver.Id, message.PartyInfo.Receiver.Type.ToString()))));
				}
				catch (Exception e)
				{
					logger.LogError(new EventId(0), e, "Error while querying available messages");
				}
			}

			return new MessageReceiveInfo(messages.ToArray());
		}

		public async Task<BusinessApiResponse<InboxMpMessage>> ReceiveMessageAsync(MpMessage mpMessage)
		{
			IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
			try
			{
				FileResponse clientResponse = await businessApiClient.V1MpMessagesInboxPayloadAsync(Guid.Parse(mpMessage.MessageId));

				using (MemoryStream ms = new MemoryStream())
				{
					await clientResponse.Stream.CopyToAsync(ms);
					byte[] zippedContent = ms.ToArray();
					ms.Position = 0;

					using (GZipStream gZipStream = new GZipStream(ms, CompressionMode.Decompress, true))
					{
						using (StreamReader decompressedReader = new StreamReader(gZipStream, DefaultEncoding))
						{
							string edifactString = await decompressedReader.ReadToEndAsync();

							return new BusinessApiResponse<InboxMpMessage>(
								true,
								new InboxMpMessage(
									mpMessage.MessageId,
									mpMessage.CreatedAt,
									mpMessage.BdewDocumentDate,
									mpMessage.PartyInfo.Sender!,
									mpMessage.PartyInfo.Receiver!,
									edifactString,
									zippedContent));
						}
					}
				}
			}
			catch (ApiException ex)
			{
				return new BusinessApiResponse<InboxMpMessage>(false,
					new InboxMpMessage(mpMessage.MessageId,
						mpMessage.CreatedAt,
						mpMessage.BdewDocumentDate,
						mpMessage.PartyInfo.Sender!,
						mpMessage.PartyInfo.Receiver!,
						null,
						null)
					,
					(HttpStatusCode)ex.StatusCode,
					ex);
			}
		}

		public async Task<BusinessApiResponse<bool>> AcknowledgeReceivedMessageAsync(InboxMpMessage mpMessage)
		{
			string tokenString = jwtBuilder.CreateSignedToken(mpMessage);
			IBusinessApiClient businessApiClient = businessApiClientFactory.Create(new Uri(as4BusinessApiEndpoint), httpClient);
			try
			{
				if (mpMessage.MessageId == null)
				{
					throw new InvalidOperationException("The message does not have a MessageId.");
				}

				await businessApiClient.V1MpMessagesInboxAcknowledgementAsync(Guid.Parse(mpMessage.MessageId), new MessageAcknowledgedRequestDto { Jwt = tokenString });

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
			httpClientHandler.Dispose();
		}

		private HttpClient InitializeHttpClient()
		{
#pragma warning disable CA5386 // Hartcodierung des SecurityProtocolType-Werts vermeiden
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore CA5386 // Hartcodierung des SecurityProtocolType-Werts vermeiden
			return new HttpClient(httpClientHandler)
			{
				BaseAddress = new Uri(as4BusinessApiEndpoint)
			};
		}

		private static HttpClientHandler InitializeHttpClientHandler(IClientCertificate clientCertificate)
		{
#pragma warning disable CA5398 // Avoid hardcoding SslProtocols values
			HttpClientHandler httpClientHandler = new HttpClientHandler
												{
													DefaultProxyCredentials = null,
													UseCookies = false,
													ClientCertificateOptions = ClientCertificateOption.Manual,
													AutomaticDecompression = DecompressionMethods.None,
													UseProxy = false,
													Proxy = null,
													PreAuthenticate = false,
													UseDefaultCredentials = false,
													Credentials = null,
													AllowAutoRedirect = false,
													SslProtocols = SslProtocols.Tls12,
													CheckCertificateRevocationList = false
												};
#pragma warning restore CA5398 // Avoid hardcoding SslProtocols values

			httpClientHandler.ClientCertificates.Add(clientCertificate.AsX509Certificate());

			return httpClientHandler;
		}
	}
}