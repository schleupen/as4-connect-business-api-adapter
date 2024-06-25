// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class As4BusinessApiClientFactory(
		IOptions<AdapterOptions> options,
		IJwtHelper jwtHelper,
		IMarketpartnerCertificateProvider marketpartnerCertificateProvider,
		ILogger<As4BusinessApiClient> clientLogger,
		IClientWrapperFactory clientWrapperFactory)
		: IAs4BusinessApiClientFactory
	{
		public IAs4BusinessApiClient CreateAs4BusinessApiClient(string marktpartnerId)
		{
			string as4BusinessApiEndpoint = options.Value.As4ConnectEndpoint;
			if (string.IsNullOrEmpty(as4BusinessApiEndpoint))
			{
				throw new CatastrophicException("The endpoint for AS4 connect is not configured.");
			}

			return new As4BusinessApiClient(jwtHelper, marketpartnerCertificateProvider, as4BusinessApiEndpoint, marktpartnerId, clientLogger, clientWrapperFactory);
		}
	}
}
