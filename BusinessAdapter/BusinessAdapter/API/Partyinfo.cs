// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Information about the receving and sending party of an AS4 message.
	/// </summary>
	public sealed class Partyinfo
	{
		public Partyinfo(SendingParty? sender, ReceivingParty? receiver)
		{
			Sender = sender;
			Receiver = receiver;
		}

		/// <summary>
		/// Sending party of the AS4 message.
		/// </summary>
		public SendingParty? Sender { get; }

		/// <summary>
		/// Receiving party of the AS4 message.
		/// </summary>
		public ReceivingParty? Receiver { get; }
	}
}