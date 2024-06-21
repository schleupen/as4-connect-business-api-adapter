// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	using System.Security.Cryptography.X509Certificates;

	public interface IConfigurationAccess
	{
		StoreLocation GetCertificateStoreLocation();

		StoreName GetCertificateStoreName();

		string ResolveBusinessApiEndpoint();

		string ReadReceiveDirectory();

		string ReadSendDirectory();

		IReadOnlyCollection<string> ReadOwnMarketpartners();

		int ReceivingRetryCount { get; }

		int ReceivingMessageLimitCount { get; }

		int DeliveryRetryCount { get; }

		int DeliveryMessageLimitCount { get; }
	}
}
