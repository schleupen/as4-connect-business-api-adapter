namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Sending;
using Schleupen.AS4.BusinessAdapter.API;

public class FpFile : IFpFile
{
    private readonly byte[] content;
    private readonly string filename;
    private readonly string bdewDocumentNo;
    private readonly string bdewDocumentType;
    private readonly string bdewFulfillmentDate;
    private readonly string bdewSubjectPartyId;
    private readonly string bdewSubjectPartyRole;
    private readonly string receiverIdentificationNumber;
    private readonly string receiverIdentificationNumberType;

    public FpFile(
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
    {
        this.content = content;
        this.filename = filename;
        this.bdewDocumentNo = bdewDocumentNo;
        this.bdewDocumentType = bdewDocumentType;
        this.bdewFulfillmentDate = bdewFulfillmentDate;
        this.bdewSubjectPartyId = bdewSubjectPartyId;
        this.bdewSubjectPartyRole = bdewSubjectPartyRole;
        this.Path = path;
        this.SenderIdentificationNumber = senderIdentificationNumber;
        this.receiverIdentificationNumber = receiverIdentificationNumber;
        this.receiverIdentificationNumberType = receiverIdentificationNumberType;
    }

    public string Path { get; }
    
    public string? SenderIdentificationNumber { get; }
    
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