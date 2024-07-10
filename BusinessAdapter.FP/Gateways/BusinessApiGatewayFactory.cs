// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;

	public sealed class BusinessApiGatewayFactory(
		IHttpClientFactory httpClientFactory,
		IBusinessApiClientFactory businessApiClientFactory,
		IPartyIdTypeAssembler partyIdTypeAssembler,
		ILogger<BusinessApiGateway> logger)
		: IBusinessApiGatewayFactory
	{
		public IBusinessApiGateway CreateGateway(Party party)
		{
			return new BusinessApiGateway(party, httpClientFactory, businessApiClientFactory, partyIdTypeAssembler, logger);
		}
	}
}