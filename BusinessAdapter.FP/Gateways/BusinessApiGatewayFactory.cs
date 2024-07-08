// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;

	public sealed class BusinessApiGatewayFactory(
		IBusinessApiClientFactory businessApiClientFactory,
		IHttpClientFactory httpClientFactory,
		IPartyIdTypeAssembler partyIdTypeAssembler,
		ILogger<BusinessApiGateway> logger)
		: IBusinessApiGatewayFactory
	{
		public IBusinessApiGateway CreateApiGateway(Party party)
		{
			return new BusinessApiGateway(httpClientFactory.CreateHttpClientFor(party), businessApiClientFactory, partyIdTypeAssembler, logger);
		}
	}
}