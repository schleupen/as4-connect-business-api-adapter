namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

/// <summary>
/// Incoming AS4 message with metadata and XML payload
/// </summary>
public sealed class As4FpMessage
{
    public As4FpMessage(
        DateTimeOffset createdAt, 
        string messageId, 
        PartyInfo partyInfo, 
        string bdewDocumentNo,
        string bdewFulfillmentDate, 
        string bdewSubjectPartyId,
        string bdewSubjectPartyRole,
        string bdewDocumentType)
    {
        CreatedAt = createdAt;
        MessageId = messageId;
        PartyInfo = partyInfo;
        BdewDocumentNo = bdewDocumentNo;
        BdewFulfillmentDate = bdewFulfillmentDate;
        BdewSubjectPartyId = bdewSubjectPartyId;
        BdewSubjectPartyRole = bdewSubjectPartyRole;
        BDEWDocumentType = bdewDocumentType;
    }
    
    /// <summary>
    /// The identification of the message.
    /// </summary>
    public string MessageId { get; }

    /// <summary>
    /// The timestamp of creation in AS4 Connect.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }
    
    /// <summary>
    /// The document no of the xml message (in the context of FP the version of the message)
    /// </summary>
    public string BdewDocumentNo { get; }

    /// <summary>
    /// 
    /// </summary>
    public string BdewFulfillmentDate { get; }
    
    /// <summary>
    /// Eine Senderidentifikation gemäß Coding Scheme, z. B. A01.
    /// </summary>
    public string BdewSubjectPartyId { get; }

    /// <summary>
    /// Ein Code für die Senderrole, z. B. A08 (bei Schedule Messages) oder A04 (ACK, CNF oder ANO).
    /// </summary>
    public string BdewSubjectPartyRole { get; }
    
    /// <summary>
    /// A01 für Fahrplananmeldungen. A17 für ACK. A16 für ANO. A07, A08 oder A09 für CNF.
    /// </summary>
    public string BDEWDocumentType { get; set; } = default!;
    
    /// <summary>
    /// Contains information about the sending and receiving party of the message.
    /// </summary>
    public PartyInfo PartyInfo { get; }
}