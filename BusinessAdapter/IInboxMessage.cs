namespace Schleupen.AS4.BusinessAdapter;

using Schleupen.AS4.BusinessAdapter.API;

public interface IInboxMessage
{
	/// <summary>
	/// The Identifier of the message.
	/// </summary>
	string MessageId { get; }

	/// <summary>
	/// The sending party of the AS4 message.
	/// </summary>
	SendingParty Sender { get; }

	/// <summary>
	/// The receiving party of the AS4 message.
	/// </summary>
	ReceivingParty Receiver { get; }

	string? ContentHashSha256 { get; }
}