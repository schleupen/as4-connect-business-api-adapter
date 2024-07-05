// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;

	/// <summary>
	/// A X509 certificate store.
	/// </summary>
	public sealed class ClientCertificateStore : IClientCertificateStore
	{
		private readonly X509Store x509Store;

		/// <summary>
		/// Returns the available AS4 certificates in the certificate store.
		/// </summary>
		public IReadOnlyCollection<IClientCertificate> Certificates
		{
			get
			{
				return x509Store.Certificates.Where(IsDistinguishedNameAs4).Select(x => new ClientCertificate(x)).ToList();
			}
		}

		public ClientCertificateStore(X509Store x509Store)
		{
			this.x509Store = x509Store;
		}

		/// <summary>
		/// Disposes the certificate store.
		/// </summary>
		public void Dispose()
		{
			x509Store.Dispose();
		}

		private bool IsDistinguishedNameAs4(X509Certificate2? certificate)
		{
			return certificate?.IsSubjectDistinguishedNameEqualToAs4() ?? false;
		}
	}
}