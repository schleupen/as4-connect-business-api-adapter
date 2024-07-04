namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

using Schleupen.AS4.BusinessAdapter.API;

public class FpOutboxMessage(
	SendingParty sendingParty,
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

    public string BDEWDocumentType { get;} = bdewDocumentType;

    /// <summary>
    /// Datenaustauschreferenz (DAR) aus UNB DE0020
    /// </summary>
    public string BDEWDocumentNo { get; } = bdewDocumentNo;

    /// <summary>
    /// Das geplante Zeitintervall.
    /// </summary>
    public string BDEWFulfillmentDate { get; } = bdewFulfillmentDate;

    /// <summary>
    /// Eine Senderidentifikation gemäß Coding Scheme, z. B. A01.
    /// </summary>
    public string BDEWSubjectPartyId { get; } = bdewSubjectPartyId;

    /// <summary>
    /// Ein Code für die Senderrole, z. B. A08 (bei Schedule Messages) oder A04 (ACK, CNF oder ANO).
    /// </summary>
    public string BDEWSubjectPartyRole { get; } = bdewSubjectPartyRole;

    /// <summary>
    /// Optional Id which the sender of the message may individually add. This Id should originate from the connected bussiness application and allows to identify the message in AS4 Connect later on.
    /// </summary>
    public string? SenderMessageId { get; } = senderMessageId;

    /// <summary>
    /// The file name of the XML file.
    /// </summary>
    public string FileName { get; } = filename;
}