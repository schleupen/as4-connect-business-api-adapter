namespace Schleupen.AS4.BusinessAdapter.FP;

public interface IFpFile
{
	string Path { get; }
	EIC Sender { get; }
	EIC Receiver { get; }
	string BDEWDocumentType { get; }
	string BDEWDocumentNo { get; }
	string BDEWFulfillmentDate { get; }
	string BDEWSubjectPartyId { get; }
	string BDEWSubjectPartyRole { get; }
	byte[] Content { get; }
}