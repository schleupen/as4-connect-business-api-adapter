// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Receiver of an AS4 message.
	/// </summary>
	public sealed record ReceivingParty(string id, string type) : Party(id, type)
	{
	}
}
