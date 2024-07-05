// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	/// <summary>
	/// A certificate store.
	/// </summary>
	public interface IClientCertificateStore : IDisposable
	{
		/// <summary>
		/// Returns the available AS4 certificates in the store.
		/// </summary>
		IReadOnlyCollection<IClientCertificate> Certificates { get; }
	}
}