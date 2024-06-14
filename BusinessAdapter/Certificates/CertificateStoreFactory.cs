// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	/// <summary>
	/// Factory for certificate stores.
	/// </summary>
	public sealed class CertificateStoreFactory : ICertificateStoreFactory
	{
		private readonly IConfigurationAccess configuration;

		public CertificateStoreFactory(IConfigurationAccess configuration)
		{
			this.configuration = configuration;
		}

		/// <summary>
		/// Creates a new certificate store and opens it.
		/// </summary>
		/// <returns>A certificate store.</returns>
		public ICertificateStore CreateAndOpen()
		{
			StoreName storeName = configuration.GetCertificateStoreName();
			StoreLocation storeLocation = configuration.GetCertificateStoreLocation();

			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);
			return new CertificateStore(store);
		}
	}
}