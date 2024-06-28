namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Sending;
using Schleupen.AS4.BusinessAdapter.API;

public class FpFile(
	byte[] content,
	string filename,
	string bdewDocumentNo,
	string bdewDocumentType,
	string bdewFulfillmentDate,
	string bdewSubjectPartyId,
	string bdewSubjectPartyRole,
	string path,
	string? senderIdentificationNumber,
	string receiverIdentificationNumber,
	string receiverIdentificationNumberType)
	: IFpFile
{
	public string Path { get; } = path;

    public string? SenderIdentificationNumber { get; } = senderIdentificationNumber;

    public FpOutboxMessage CreateOutboxMessage()
    {
        return new FpOutboxMessage(
            new ReceivingParty(receiverIdentificationNumber, receiverIdentificationNumberType),
            string.Empty,
            content,
            filename,
            bdewDocumentNo,
            bdewDocumentType,
            bdewFulfillmentDate,
            bdewSubjectPartyId,
            bdewSubjectPartyRole
            );
    }
}