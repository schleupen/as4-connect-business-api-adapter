// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class BusinessApiGatewayFactory(
		IHttpClientFactory httpClientFactory,
		IBusinessApiClientFactory businessApiClientFactory,
		IPartyIdTypeAssembler partyIdTypeAssembler,
		ILogger<BusinessApiGateway> logger, 
		IOptions<AdapterOptions> options,
		IJwtBuilder jwtBuilder)
		: IBusinessApiGatewayFactory
	{
		private readonly string as4BusinessApiEndpoint = options.Value.As4ConnectEndpoint;
		public IBusinessApiGateway CreateGateway(FpParty party)
		{
			return new BusinessApiGateway(party, httpClientFactory, businessApiClientFactory, partyIdTypeAssembler, as4BusinessApiEndpoint, logger, jwtBuilder);
		}
	}
}