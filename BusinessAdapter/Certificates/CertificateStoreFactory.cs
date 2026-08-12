// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	/// <summary>
	/// Factory for certificate stores.
	/// </summary>
	public sealed class CertificateStoreFactory(IOptions<AdapterOptions> options) : ICertificateStoreFactory
	{
		/// <summary>
		/// Creates a new certificate store and opens it.
		/// </summary>
		/// <returns>A certificate store.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = "Disposed by receiver.")]
		public IClientCertificateStore CreateAndOpen()
		{
			StoreName storeName = options.Value.CertificateStoreName;
			StoreLocation storeLocation = options.Value.CertificateStoreLocation;

			X509Store store = new(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);
			return new ClientCertificateStore(store);
		}
	}
}