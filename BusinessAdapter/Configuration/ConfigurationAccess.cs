// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.API;

	public sealed class ConfigurationAccess : IConfigurationAccess
	{
		private readonly AdapterOptions adapterOptions;

		public ConfigurationAccess(IOptions<AdapterOptions> adapterOptions, ILogger<ConfigurationAccess> logger)
		{
			this.adapterOptions = adapterOptions.Value;

			logger.LogInformation("Current configuration: {ConfigurationSummary}", BuildConfigurationSummary());
		}

		private string BuildConfigurationSummary()
		{
			string marketpartnerSummary = adapterOptions.Marketpartners != null && adapterOptions.Marketpartners.Length > 0 ? string.Join(", ", adapterOptions.Marketpartners) : "None";
			return $"Marketpartners: {marketpartnerSummary}\n\tSend directory: {adapterOptions.SendDirectory}\n\tReceive directory: {adapterOptions.ReceiveDirectory}";
		}

		public string ReadReceiveDirectory()
		{
			return adapterOptions.ReceiveDirectory;
		}

		public string ResolveBusinessApiEndpoint()
		{
			return adapterOptions.As4ConnectEndpoint;
		}

		public string ReadSendDirectory()
		{
			return adapterOptions.SendDirectory;
		}

		public IReadOnlyCollection<string> ReadOwnMarketpartners()
		{
			var marketpartners = new List<string>();

			var marketpartnersFromConfiguration = adapterOptions.Marketpartners ?? Array.Empty<string>();
			foreach (string marketpartnerFromConfiguration in marketpartnersFromConfiguration)
			{
				marketpartners.Add(marketpartnerFromConfiguration);
			}

			return marketpartners;
		}

		public StoreName GetCertificateStoreName()
		{
			var certificateStoreNameString = adapterOptions.CertificateStoreName;

			if (!Enum.TryParse(certificateStoreNameString, true, out StoreName certificateStoreName))
			{
				throw new CatastrophicException("Could not parse certificate store name. Please use one of the store names available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0.");
			}

			return certificateStoreName;
		}

		public StoreLocation GetCertificateStoreLocation()
		{
			var certificateStoreLocationString = adapterOptions.CertificateStoreLocation;

			if (!Enum.TryParse(certificateStoreLocationString, true, out StoreLocation certificateStoreLocation))
			{
				throw new CatastrophicException("Could not parse certificate store location. Please use one of the store locations available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0.");
			}

			return certificateStoreLocation;
		}

		public int ReceivingRetryCount => this.adapterOptions.ReceivingRetryCount;

		public int ReceivingMessageLimitCount => this.adapterOptions.ReceivingMessageLimitCount;

		public int DeliveryRetryCount => this.adapterOptions.DeliveryRetryCount;

		public int DeliveryMessageLimitCount => this.adapterOptions.DeliveryMessageLimitCount;
	}
}