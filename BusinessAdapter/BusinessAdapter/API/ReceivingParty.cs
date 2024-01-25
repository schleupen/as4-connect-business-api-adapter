// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Receiver of an AS4 message.
	/// </summary>
	public sealed class ReceivingParty
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">The identification number.</param>
		/// <param name="type">The name of the code providing authority of the identification number. (e.g. BDEW)</param>
		public ReceivingParty(string id, string type)
		{
			Id = id;
			Type = type;
		}

		/// <summary>
		/// Identification number of the receiver.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Code providing authority of the identification e.g. BDEW.
		/// </summary>
		public string Type { get; }
	}
}
