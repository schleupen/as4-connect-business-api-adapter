namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

/// <summary>
/// Inbound FP Message.
/// </summary>
public sealed class FpInboxMessage(
	Guid messageId,
	SendingParty sender,
	ReceivingParty receiver,
	DateTimeOffset createdAt,
	FpBDEWProperties fpBDEWProperties)
{
	/// <summary>
    /// The identification of the message.
    /// </summary>
    public Guid MessageId { get; } = messageId;

    /// <summary>
    /// The timestamp of creation in AS4 Connect.
    /// </summary>
    public DateTimeOffset CreatedAt { get; } = createdAt;

    /// <summary>
    /// AS4 specific required Propereties
    /// </summary>
    public FpBDEWProperties BDEWProperties { get; } = fpBDEWProperties;

    /// <summary>
    /// Receiving party of the message.
    /// </summary>
    public SendingParty Sender { get; } = sender;

    /// <summary>
    /// Receiving party of the message.
    /// </summary>
    public ReceivingParty Receiver { get; } = receiver;

}