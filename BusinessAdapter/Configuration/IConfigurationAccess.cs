// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	using System.Security.Cryptography.X509Certificates;

	public interface IConfigurationAccess
	{
		StoreLocation GetCertificateStoreLocation();

		StoreName GetCertificateStoreName();

		string ResolveBusinessApiEndpoint();

		IReadOnlyCollection<string> ReadOwnMarketpartners();
	}
}
