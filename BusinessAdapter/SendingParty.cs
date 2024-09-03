// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	/// <summary>
	/// Sending party of an AS4 message.
	/// </summary>
	public sealed record SendingParty(string Id, string Type) : Party(Id, Type)
	{
	}
}