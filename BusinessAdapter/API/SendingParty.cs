// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Sending party of an AS4 message.
	/// </summary>
	public sealed class SendingParty
	{
		public SendingParty(string id)
		{
			Id = id;
		}

		/// <summary>
		/// The identification number.
		/// </summary>
		public string Id { get; }
	}
}