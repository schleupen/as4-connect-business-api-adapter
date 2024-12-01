namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

public sealed record FpBDEWProperties(
	string BDEWDocumentType,
	string BDEWDocumentNo,
	string BDEWFulfillmentDate,
	string BDEWSubjectPartyId,
	string BDEWSubjectPartyRole)
{
	/// <summary>
	/// Der Dokument-Typ (z.B: A01, A17 ...)
	/// </summary>
	public string BDEWDocumentType { get; } = BDEWDocumentType;

	/// <summary>
	/// Version (z.B: MessageVersion, ReceivingMessageVersion )
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
		return BDEWDocumentType switch
		{
			BDEWDocumentTypes.A01 => FpMessageType.Schedule,
			BDEWDocumentTypes.A07 or BDEWDocumentTypes.A08 or BDEWDocumentTypes.A09 => FpMessageType.ConfirmationReport,
			BDEWDocumentTypes.A17 => FpMessageType.Acknowledge,
			BDEWDocumentTypes.A16 => FpMessageType.AnomalyReport,
			BDEWDocumentTypes.A59 => // prozessbeschreibung_fahrplananmeldung_v4.5 in A.4.1.1
				FpMessageType.StatusRequest,
			_ => throw new NotSupportedException($"Document type '{BDEWDocumentType}' is not supported.")
		};
	}
}