// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	using System.Security.Cryptography.X509Certificates;

	public interface IConfigurationAccess
	{
		AdapterConfiguration ReadAdapterConfigurationValue();

		StoreLocation GetCertificateStoreLocation();

		StoreName GetCertificateStoreName();

		string ResolveBusinessApiEndpoint();

		string ReadReceiveDirectory();

		string ReadSendDirectory();

		IReadOnlyCollection<string> ReadOwnMarketpartners();
	}
}
