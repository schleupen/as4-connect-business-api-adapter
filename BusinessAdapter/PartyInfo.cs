// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	/// <summary>
	/// Information about the receiving and sending party of an AS4 message.
	/// </summary>
	public sealed class PartyInfo(SendingParty? sender, ReceivingParty? receiver)
	{
		/// <summary>
		/// Sending party of the AS4 message.
		/// </summary>
		public SendingParty? Sender { get; } = sender;

		/// <summary>
		/// Receiving party of the AS4 message.
		/// </summary>
		public ReceivingParty? Receiver { get; } = receiver;
	}
}