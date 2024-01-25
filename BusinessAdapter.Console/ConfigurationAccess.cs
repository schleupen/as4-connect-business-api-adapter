// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class ConfigurationAccess : IConfigurationAccess
	{
		private readonly IOptions<AdapterOptions> adapterOptions;

		public ConfigurationAccess(IOptions<AdapterOptions> adapterOptions, ILogger<ConfigurationAccess> logger)
		{
			this.adapterOptions = adapterOptions;

			logger.LogInformation("Current configuration: {ConfigurationSummary}", BuildConfigurationSummary());
		}

		private string BuildConfigurationSummary()
		{
			string marketpartnerSummary = adapterOptions.Value.Marketpartners != null && adapterOptions.Value.Marketpartners.Length > 0 ? string.Join(", ", adapterOptions.Value.Marketpartners) : "None";
			return $"Marketpartners: {marketpartnerSummary}\n\tSend directory: {adapterOptions.Value.SendDirectory}\n\tReceive directory: {adapterOptions.Value.ReceiveDirectory}";
		}

		public AdapterConfiguration ReadAdapterConfigurationValue()
		{
			return new AdapterConfiguration(
				receivingMessageLimitCount: adapterOptions.Value.ReceivingMessageLimitCount,
				deliveryMessageLimitCount: adapterOptions.Value.DeliveryMessageLimitCount,
				deliveryRetryCount: adapterOptions.Value.DeliveryRetryCount,
				receivingRetryCount: adapterOptions.Value.ReceivingRetryCount);
		}

		public string ReadReceiveDirectory()
		{
			return adapterOptions.Value.ReceiveDirectory;
		}

		public string ResolveBusinessApiEndpoint()
		{
			return adapterOptions.Value.As4ConnectEndpoint;
		}

		public string ReadSendDirectory()
		{
			return adapterOptions.Value.SendDirectory;
		}

		public IReadOnlyCollection<string> ReadOwnMarketpartners()
		{
			var marketpartners = new List<string>();

			var marketpartnersFromConfiguration = adapterOptions.Value.Marketpartners ?? Array.Empty<string>();
			foreach (string marketpartnerFromConfiguration in marketpartnersFromConfiguration)
			{
				marketpartners.Add(marketpartnerFromConfiguration);
			}

			return marketpartners;
		}

		public StoreName GetCertificateStoreName()
		{
			var certificateStoreNameString = adapterOptions.Value.CertificateStoreName;

			if (!Enum.TryParse(certificateStoreNameString, true, out StoreName certificateStoreName))
			{
				throw new CatastrophicException("Could not parse certificate store name. Please use one of the store names available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0.");
			}

			return certificateStoreName;
		}

		public StoreLocation GetCertificateStoreLocation()
		{
			var certificateStoreLocationString = adapterOptions.Value.CertificateStoreLocation;

			if (!Enum.TryParse(certificateStoreLocationString, true, out StoreLocation certificateStoreLocation))
			{
				throw new CatastrophicException("Could not parse certificate store location. Please use one of the store locations available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0.");
			}

			return certificateStoreLocation;
		}
	}
}