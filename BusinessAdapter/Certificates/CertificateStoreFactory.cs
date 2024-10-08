﻿// Copyright...:  (c)  Schleupen SE

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
		public IClientCertificateStore CreateAndOpen()
		{
			StoreName storeName = options.Value.CertificateStoreName;
			StoreLocation storeLocation = options.Value.CertificateStoreLocation;

			X509Store store = new X509Store(storeName, storeLocation);
			store.Open(OpenFlags.ReadOnly);
			return new ClientCertificateStore(store);
		}
	}
}