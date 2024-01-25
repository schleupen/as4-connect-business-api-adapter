// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	/// <summary>
	/// Factory for certificate stores.
	/// </summary>
	public interface ICertificateStoreFactory
	{
		/// <summary>
		/// Creates a new access to the certificate store and opens it.
		/// </summary>
		/// <returns>The certificate store.</returns>
		ICertificateStore CreateAndOpen();
	}
}