// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Runtime.InteropServices;
	using System.Security.Cryptography.X509Certificates;

	/// <summary>
	/// A X509 certificate store.
	/// </summary>
	public sealed class ClientCertificateStore(X509Store x509Store) : IClientCertificateStore
	{
		/// <summary>
		/// Returns the available AS4 certificates in the certificate store.
		/// </summary>
		public IReadOnlyCollection<IClientCertificate> Certificates
		{
			get
			{
				return x509Store.Certificates
					.Cast<X509Certificate2>()
					.Where(IsDistinguishedNameAs4)
					.Select(x => new ClientCertificate(x)).ToList();
			}
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