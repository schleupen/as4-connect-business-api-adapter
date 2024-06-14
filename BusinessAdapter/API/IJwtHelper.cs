// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	/// <summary>
	/// Helper for the creation of JWT.
	/// </summary>
	public interface IJwtHelper
	{
		/// <summary>
		/// Creates a signed JWT.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <returns>The signed token for the given message.</returns>
		string CreateSignedToken(InboxMessage message);
	}
}