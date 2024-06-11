// Copyright...:  (c)  Schleupen SE

// Generate ApiClient and Contracts with NSwag.
// Required settings can be found in edi.as4.Gateways.Contracts.BusinessApi.snwag.

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.IO.Compression;
	using System.Net;
	using System.Net.Http;
	using System.Net.Security;
	using System.Security.Authentication;
	using System.Security.Cryptography.X509Certificates;
	using System.Text;
	using System.Threading.Tasks;
	using BusinessApi;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Receiving;
	using Schleupen.AS4.BusinessAdapter.Sending;

	public sealed class As4BusinessApiClient : IAs4BusinessApiClient
	{
		private static readonly Encoding DefaultEncoding = Encoding.GetEncoding("ISO-8859-1");

		private readonly IJwtHelper jwtHelper;
		private readonly string as4BusinessApiEndpoint;
		private readonly ILogger<As4BusinessApiClient> logger;
		private readonly IClientWrapperFactory clientWrapperFactory;
		private readonly HttpClient httpClient;
		private readonly HttpClientHandler httpClientHandler;

		public As4BusinessApiClient(
			IJwtHelper jwtHelper,
			IMarketpartnerCertificateProvider marketpartnerCertificateProvider,
			string as4BusinessApiEndpoint,
			string marketpartnerIdentification,
			ILogger<As4BusinessApiClient> logger,
			IClientWrapperFactory clientWrapperFactory)
		{
			this.jwtHelper = jwtHelper;
			this.as4BusinessApiEndpoint = as4BusinessApiEndpoint;
			this.logger = logger;
			this.clientWrapperFactory = clientWrapperFactory;

			IAs4Certificate resolvedMarketpartnerCertificate = marketpartnerCertificateProvider.GetMarketpartnerCertificate(marketpartnerIdentification);
			httpClientHandler = InitializeHttpClientHandler(resolvedMarketpartnerCertificate);
			httpClient = InitializeHttpClient();
		}

		public async Task<MessageResponse<OutboxMessage>> SendMessageAsync(OutboxMessage message)
		{
			using (MemoryStream compressedStream = new MemoryStream())
			{
				using (MemoryStream payloadStream = new MemoryStream(message.Payload))
				using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
				{
					await payloadStream.CopyToAsync(gZipStream);
				}

				compressedStream.Position = 0;
				IClientWrapper client = clientWrapperFactory.Create(as4BusinessApiEndpoint, httpClient);
				try
				{
					await client.V1MpMessagesOutboxPostAsync(message.Receiver.Id,
						ToPartyTypeDto(message.Receiver.Type),
						new FileParameter(compressedStream, message.FileName),
						message.BdewDocumentType,
						message.BdewDocumentNumber,
						message.DocumentDate.ToString("yyyy-MM-dd"),
						Guid.NewGuid(),
						message.SenderMessageId);

					return new MessageResponse<OutboxMessage>(true, message);
				}
				catch (ApiException ex)
				{
					return new MessageResponse<OutboxMessage>(false, message, (HttpStatusCode)ex.StatusCode, ex);
				}
			}
		}

		public async Task<MessageReceiveInfo> QueryAvailableMessagesAsync(int limit = 50)
		{
			IClientWrapper client = clientWrapperFactory.Create(as4BusinessApiEndpoint, httpClient);
			QueryInboxMessagesResponseDto clientResponse = await client.V1MpMessagesInboxAsync(limit);

			List<As4Message> messages = new List<As4Message>();

			foreach (InboundMPMessageDto? message in clientResponse.Messages)
			{
				try
				{
					if (message.PartyInfo == null)
					{
						throw new ArgumentException($"The message with the identification {message.MessageId} has no party info.");
					}

					messages.Add(new As4Message(
						message.Created_at,
						message.BdewDocumentDate,
						message.MessageId.ToString(),
						new Partyinfo(new SendingParty(message.PartyInfo.Sender.Id), new ReceivingParty(message.PartyInfo.Receiver.Id, message.PartyInfo.Receiver.Type.ToString()))));
				}
				catch (Exception e)
				{
					logger.LogError(new EventId(0), e, "Error while querying available messages");
				}
			}

			return new MessageReceiveInfo(messages.ToArray());
		}

		public async Task<MessageResponse<InboxMessage>> ReceiveMessageAsync(As4Message as4Message)
		{
			IClientWrapper client = clientWrapperFactory.Create(as4BusinessApiEndpoint, httpClient);
			try
			{
				FileResponse clientResponse = await client.V1MpMessagesInboxPayloadAsync(Guid.Parse(as4Message.MessageId));

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

							return new MessageResponse<InboxMessage>(
								true,
								new InboxMessage(
									as4Message.MessageId,
									as4Message.CreatedAt,
									as4Message.BdewDocumentDate,
									as4Message.PartyInfo.Sender!,
									as4Message.PartyInfo.Receiver!,
									edifactString,
									zippedContent));
						}
					}
				}
			}
			catch (ApiException ex)
			{
				return new MessageResponse<InboxMessage>(true,
					new InboxMessage(as4Message.MessageId,
						as4Message.CreatedAt,
						as4Message.BdewDocumentDate,
						as4Message.PartyInfo.Sender!,
						as4Message.PartyInfo.Receiver!,
						null,
						null)
					,
					(HttpStatusCode)ex.StatusCode,
					ex);
			}
		}

		public async Task<MessageResponse<bool>> AcknowledgeReceivedMessageAsync(InboxMessage message)
		{
			string tokenString = jwtHelper.CreateSignedToken(message);
			IClientWrapper client = clientWrapperFactory.Create(as4BusinessApiEndpoint, httpClient);
			try
			{
				if (message.MessageId == null)
				{
					throw new InvalidOperationException("The message does not have a MessageId.");
				}

				await client.V1MpMessagesInboxAcknowledgementAsync(Guid.Parse(message.MessageId), new MessageAcknowledgedRequestDto { Jwt = tokenString });

				return new MessageResponse<bool>(true, true);
			}
			catch (ApiException ex)
			{
				return new MessageResponse<bool>(false, false, (HttpStatusCode)ex.StatusCode, ex);
			}
		}

		public void Dispose()
		{
			httpClient.Dispose();
			httpClientHandler.Dispose();
		}

		private PartyIdTypeDto? ToPartyTypeDto(string? foreignMarketpartnerType)
		{
			switch (foreignMarketpartnerType?.ToUpperInvariant())
			{
				case null:
					return null;
				case "BDEW":
					return PartyIdTypeDto.BDEW;
				case "DVGW":
					return PartyIdTypeDto.DVGW;
				case "GS1":
				case "GS1GERMANY":
					return PartyIdTypeDto.GS1;
				default:
					throw new NotSupportedException(nameof(foreignMarketpartnerType));
			}
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

		private static HttpClientHandler InitializeHttpClientHandler(IAs4Certificate as4Certificate)
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
													//ClientCertificates = { clientMarketpartner.Certificate.AsX509Certificate() },
													CheckCertificateRevocationList = false
												};
#pragma warning restore CA5398 // Avoid hardcoding SslProtocols values

			httpClientHandler.ClientCertificates.Add(as4Certificate.AsX509Certificate());

			httpClientHandler.ServerCertificateCustomValidationCallback = Test;

			return httpClientHandler;
		}

		private static bool Test(HttpRequestMessage arg1, X509Certificate2? arg2, X509Chain? arg3, SslPolicyErrors arg4)
		{
			return true;
		}
	}
}