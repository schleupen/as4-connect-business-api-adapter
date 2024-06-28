namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class InboxFpMessage : IInboxMessage
{
    public string? MessageId { get; }
    public Schleupen.AS4.BusinessAdapter.API.SendingParty Sender { get; }
    public Schleupen.AS4.BusinessAdapter.API.ReceivingParty Receiver { get; }
    public string? ContentHashSha256 { get; }
}