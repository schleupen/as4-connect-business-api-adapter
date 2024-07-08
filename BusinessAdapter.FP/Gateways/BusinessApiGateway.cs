// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using System.IO;
	using System.IO.Compression;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;
	using Schleupen.AS4.BusinessAdapter.FP.Sending;

	public sealed class BusinessApiGateway(
		HttpClient httpClient,
		IBusinessApiClientFactory businessApiClientFactory,
		IPartyIdTypeAssembler partyIdTypeAssembler,
		ILogger<BusinessApiGateway> logger) : IBusinessApiGateway
	{
		public async Task<MessageResponse<FpOutboxMessage>> SendMessageAsync(FpOutboxMessage message)
		{
			logger.LogInformation("Sending {MessageId}", message.MessageId);
			using (MemoryStream compressedStream = new MemoryStream())
			{
				using (MemoryStream payloadStream = new MemoryStream(message.Payload))
				using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
				{
					await payloadStream.CopyToAsync(gZipStream);
				}

				compressedStream.Position = 0;
				IBusinessApiClient businessApiClient = businessApiClientFactory.Create(httpClient.BaseAddress!, httpClient);
				try
				{
					await businessApiClient.V1FpMessagesOutboxPostAsync(message.Receiver.Id,
						partyIdTypeAssembler.ToPartyTypeDto(message.Receiver.Type),
						new FileParameter(compressedStream, message.FileName),
						message.BDEWProperties.BDEWDocumentType,
						message.BDEWProperties.BDEWDocumentNo,
						message.BDEWProperties.BDEWFulfillmentDate,
						message.BDEWProperties.BDEWSubjectPartyId,
						message.BDEWProperties.BDEWSubjectPartyRole,
						message.MessageId,
						message.SenderMessageId);

					return new MessageResponse<FpOutboxMessage>(true, message);
				}
				catch (ApiException ex)
				{
					return new MessageResponse<FpOutboxMessage>(false, message, (HttpStatusCode)ex.StatusCode, ex);
				}
			}
		}

		public void Dispose()
		{
			httpClient.Dispose();
		}
	}
}