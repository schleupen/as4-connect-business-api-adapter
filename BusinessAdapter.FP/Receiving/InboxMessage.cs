namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

using Schleupen.AS4.BusinessAdapter.API;

public class InboxFpMessage : IInboxMessage
{
    public InboxFpMessage(
        string? messageId,
        SendingParty sender,
        ReceivingParty receiver,
        string? contentHashSha256,
        byte[] payload,
        string bdewDocumentType,
        string bdewDocumentNo,
        string bdewFulfillmentDate,
        string bdewSubjectPartyId,
        string bdewSubjectPartyRole,
        string? senderMessageId)
    {
        MessageId = messageId;
        Sender = sender;
        Receiver = receiver;
        ContentHashSha256 = contentHashSha256;
        Payload = payload;
        BDEWDocumentType = bdewDocumentType;
        BDEWDocumentNo = bdewDocumentNo;
        BDEWFulfillmentDate = bdewFulfillmentDate;
        BDEWSubjectPartyId = bdewSubjectPartyId;
        BDEWSubjectPartyRole = bdewSubjectPartyRole;
        SenderMessageId = senderMessageId;
    }

    public string? MessageId { get; }

    public SendingParty Sender { get; }

    public ReceivingParty Receiver { get; }

    public string? ContentHashSha256 { get; }

    /// <summary>
    /// XML payload.
    /// </summary>
#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
    public byte[] Payload { get; }
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben

    public string BDEWDocumentType { get;}

    /// <summary>
    /// Datenaustauschreferenz (DAR) aus UNB DE0020
    /// </summary>
    public string BDEWDocumentNo { get; }

    /// <summary>
    /// Das geplante Zeitintervall.
    /// </summary>
    public string BDEWFulfillmentDate { get; }

    /// <summary>
    /// Eine Senderidentifikation gemäß Coding Scheme, z. B. A01.
    /// </summary>
    public string BDEWSubjectPartyId { get; }

    /// <summary>
    /// Ein Code für die Senderrole, z. B. A08 (bei Schedule Messages) oder A04 (ACK, CNF oder ANO).
    /// </summary>
    public string BDEWSubjectPartyRole { get; }

    /// <summary>
    /// Optional Id which the sender of the message may individually add. This Id should originate from the connected bussiness application and allows to identify the message in AS4 Connect later on.
    /// </summary>
    public string? SenderMessageId { get; }

    /// <summary>
    /// The file name of the XML file.
    /// </summary>
    public string FileName { get; }
}