namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

using Schleupen.AS4.BusinessAdapter.API;

public class InboxFpMessage(
	string messageId,
	SendingParty sender,
	ReceivingParty receiver,
	string? contentHashSha256,
	byte[] payload,
	FpBDEWProperties bdewProperties)
	: IInboxMessage
{
    public SendingParty Sender { get; } = sender;

    public ReceivingParty Receiver { get; } = receiver;

    public string MessageId { get; } = messageId;

    public string? ContentHashSha256 { get; } = contentHashSha256;

    /// <summary>
    /// XML payload.
    /// </summary>
#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
    public byte[] Payload { get; } = payload;
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben

	public FpBDEWProperties BDEWProperties { get; } = bdewProperties;

    /// <summary>
    /// The file name of the XML file.
    /// </summary>
    public string FileName { get; }
}