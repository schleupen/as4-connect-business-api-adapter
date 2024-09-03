// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	/// <summary>
	/// Receiver of an AS4 message.
	/// </summary>
	public sealed record ReceivingParty(string Id, string Type) : Party(Id, Type)
	{
	}
}
