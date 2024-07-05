// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class BusinessApiGatewayFactory(
		IOptions<AdapterOptions> options,
		IJwtBuilder jwtBuilder,
		IClientCertificateProvider clientCertificateProvider,
		ILogger<BusinessApiGateway> clientLogger,
		IBusinessApiClientFactory businessApiClientFactory)
		: IBusinessApiGatewayFactory
	{
		public IBusinessApiGateway CreateAs4BusinessApiClient(string marktpartnerId)
		{
			string as4BusinessApiEndpoint = options.Value.As4ConnectEndpoint;
			if (string.IsNullOrEmpty(as4BusinessApiEndpoint))
			{
				throw new CatastrophicException("The endpoint for AS4 connect is not configured.");
			}

			return new BusinessApiGateway(jwtBuilder, clientCertificateProvider, as4BusinessApiEndpoint, marktpartnerId, businessApiClientFactory, clientLogger);
		}
	}
}
