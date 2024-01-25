// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	/// <summary>
	/// A certificate store.
	/// </summary>
	public interface ICertificateStore : IDisposable
	{
		/// <summary>
		/// Returns the available AS4 certificates in the store.
		/// </summary>
		IReadOnlyCollection<IAs4Certificate> As4Certificates { get; }
	}
}