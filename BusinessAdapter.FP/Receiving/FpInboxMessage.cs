namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

/// <summary>
/// Incoming AS4 message with metadata and XML payload
/// </summary>
public sealed class FpInboxMessage
{
    public FpInboxMessage(
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
        BDEWProperties = new FpBDEWProperties(bdewDocumentType
            , bdewDocumentNo
            , bdewFulfillmentDate
            , bdewSubjectPartyId
            , bdewSubjectPartyRole);
    }
    
    /// <summary>
    /// The identification of the message.
    /// </summary>
    public string MessageId { get; }

    /// <summary>
    /// The timestamp of creation in AS4 Connect.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }
    
    public FpBDEWProperties BDEWProperties { get; }
    
    /// <summary>
    /// Contains information about the sending and receiving party of the message.
    /// </summary>
    public PartyInfo PartyInfo { get; }
}