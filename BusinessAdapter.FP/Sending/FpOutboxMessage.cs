namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

using Schleupen.AS4.BusinessAdapter.API;

public class FpOutboxMessage
{
    public FpOutboxMessage(
        ReceivingParty receiver,
        string senderMessageId,
        byte[] payload,
        string filename, 
        string bdewDocumentNo,
        string bdewDocumentType, 
        string bdewFulfillmentDate,
        string bdewSubjectPartyId,
        string bdewSubjectPartyRole)
    {
        Receiver = receiver;
        SenderMessageId = senderMessageId;
        Payload = payload;
        FileName = filename;
        BDEWDocumentNo = bdewDocumentNo;
        BDEWDocumentType = bdewDocumentType;
        BDEWFulfillmentDate = bdewFulfillmentDate;
        BDEWSubjectPartyId = bdewSubjectPartyId;
        BDEWSubjectPartyRole = bdewSubjectPartyRole;
    }

    /// <summary>
    /// Receiving party of the message.
    /// </summary>
    public ReceivingParty Receiver { get; }

    /// <summary>
    /// EDIFACT payload.
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