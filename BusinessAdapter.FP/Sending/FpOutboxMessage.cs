namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpOutboxMessage(
	Guid messageId,
	SendingParty sender,
	ReceivingParty receiver,
	byte[] payload,
	string filename,
	string filePath,
	FpBDEWProperties bdewProperties,
	string? senderMessageId = null)
{
	public Guid MessageId { get; } = messageId;

	/// <summary>
	/// Receiving party of the message.
	/// </summary>
	public SendingParty Sender { get; } = sender;

	/// <summary>
	/// Receiving party of the message.
	/// </summary>
	public ReceivingParty Receiver { get; } = receiver;

    /// <summary>
    /// XML payload.
    /// </summary>
#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
    public byte[] Payload { get; } = payload;
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben

	public FpBDEWProperties BDEWProperties { get; } = bdewProperties;

    /// <summary>
    /// Optional Id which the sender of the message may individually add. This Id should originate from the connected bussiness application and allows to identify the message in AS4 Connect later on.
    /// </summary>
    public string? SenderMessageId { get; } = senderMessageId;

    /// <summary>
    /// The file name of the XML file.
    /// </summary>
    public string FileName { get; } = filename;

    /// <summary>
    /// The file path of the XML file.
    /// </summary>
    public string FilePath { get; } = filename;
}