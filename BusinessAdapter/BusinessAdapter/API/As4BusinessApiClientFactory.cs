// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class As4BusinessApiClientFactory : IAs4BusinessApiClientFactory
	{
		private readonly IConfigurationAccess configuration;
		private readonly IJwtHelper jwtHelper;
		private readonly IMarketpartnerCertificateProvider marketpartnerCertificateProvider;
		private readonly ILogger<As4BusinessApiClient> clientLogger;
		private readonly IClientWrapperFactory clientWrapperFactory;

		public As4BusinessApiClientFactory(IConfigurationAccess configuration,
			IJwtHelper jwtHelper,
			IMarketpartnerCertificateProvider marketpartnerCertificateProvider,
			ILogger<As4BusinessApiClient> clientLogger,
			IClientWrapperFactory clientWrapperFactory)
		{
			this.configuration = configuration;
			this.jwtHelper = jwtHelper;
			this.marketpartnerCertificateProvider = marketpartnerCertificateProvider;
			this.clientLogger = clientLogger;
			this.clientWrapperFactory = clientWrapperFactory;
		}

		public IAs4BusinessApiClient CreateAs4BusinessApiClient(string marktpartnerId)
		{
			string as4BusinessApiEndpoint = configuration.ResolveBusinessApiEndpoint();
			if (string.IsNullOrEmpty(as4BusinessApiEndpoint))
			{
				throw new CatastrophicException("The endpoint for AS4 connect is not configured.");
			}

			return new As4BusinessApiClient(jwtHelper, marketpartnerCertificateProvider, as4BusinessApiEndpoint, marktpartnerId, clientLogger, clientWrapperFactory);
		}
	}
}
