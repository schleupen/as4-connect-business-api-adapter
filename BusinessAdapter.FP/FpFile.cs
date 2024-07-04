namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Sending;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public class FpFile(
	EIC sender,
	EIC receiver,
	byte[] content,
	string filename,
	string bdewDocumentNo,
	string bdewDocumentType,
	string bdewFulfillmentDate,
	string bdewSubjectPartyId,
	string bdewSubjectPartyRole,
	string path)
	: IFpFile
{
	public string Path { get; } = path;

	public EIC Sender { get; } = sender;

	public EIC Receiver { get; } = receiver;

	public string BDEWDocumentType { get;} = bdewDocumentType;

	public string BDEWDocumentNo { get; } = bdewDocumentNo;

	public string BDEWFulfillmentDate { get; } = bdewFulfillmentDate;

	public string BDEWSubjectPartyId { get; } = bdewSubjectPartyId;

	public string BDEWSubjectPartyRole { get; } = bdewSubjectPartyRole;

	public byte[] Content { get; } = content;

	public FpOutboxMessage CreateOutboxMessage(EICMapping mapping)
    {
	    var sendingParty = mapping.GetSendingParty(sender);
	    var receivingParty = mapping.GetReceivingParty(receiver);

        return new FpOutboxMessage(
	        sendingParty,
	        receivingParty,
            content,
            filename,
            bdewDocumentNo,
            bdewDocumentType,
            bdewFulfillmentDate,
            bdewSubjectPartyId,
            bdewSubjectPartyRole);
    }
}