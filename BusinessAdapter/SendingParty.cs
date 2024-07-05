// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using Schleupen.AS4.BusinessAdapter.API;

	/// <summary>
	/// Sending party of an AS4 message.
	/// </summary>
	public sealed record SendingParty(string id, string type) : Party(id, type)
	{
	}
}