namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public sealed record FpBDEWProperties(
	string BDEWDocumentType,
	string BDEWDocumentNo,
	string BDEWFulfillmentDate,
	string BDEWSubjectPartyId,
	string BDEWSubjectPartyRole)
{
	/// <summary>
	/// Der Dokument-Typ (z.B: Confirmation, Schedule, Anomaly)
	/// </summary>
	public string BDEWDocumentType { get; } = BDEWDocumentType;

	/// <summary>
	/// Datenaustauschreferenz (DAR) aus UNB DE0020
	/// </summary>
	public string BDEWDocumentNo { get; } = BDEWDocumentNo;

	/// <summary>
	/// Das geplante Zeitintervall.
	/// </summary>
	public string BDEWFulfillmentDate { get; } = BDEWFulfillmentDate;

	/// <summary>
	/// Eine Senderidentifikation gemäß Coding Scheme, z. B. A01.
	/// </summary>
	public string BDEWSubjectPartyId { get; } = BDEWSubjectPartyId;

	/// <summary>
	/// Ein Code für die Senderrole, z. B. A08 (bei Schedule Messages) oder A04 (ACK, CNF oder ANO).
	/// </summary>
	public string BDEWSubjectPartyRole { get; } = BDEWSubjectPartyRole;
}