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
	/// should be YYYY-MM-DD
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

	public FpMessageType ToMessageType()
	{
		switch (BDEWDocumentType)
		{
			case "A01":
				return FpMessageType.Schedule;
			case "A07":
			case "A08":
			case "A09":
				return FpMessageType.Confirmation;
			case "A17":
				return FpMessageType.Acknowledge;
			case "A16":
				return FpMessageType.Anomaly;
			case "A59": // prozessbeschreibung_fahrplananmeldung_v4.5 in A.4.1.1
				return FpMessageType.Status;
			default:
				throw new NotSupportedException($"Document type '{BDEWDocumentType}' is not supported.");
		}
	}
}