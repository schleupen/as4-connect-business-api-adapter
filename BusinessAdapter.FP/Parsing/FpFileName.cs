namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Globalization;

// Acknowledgement (Schedule):		<JJJJMMDD>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-ÜNB>_<VVV>_ACK_<YYYYMM-DDTHH-MM-SSZ>.XML
// Acknowledgement (StatusRequest):	<JJJJMMDD>_SRQ_<EIC-NAME-BILANZKREIS>_<EIC-NAME-ÜNB>_ACK_<YYYY-MMDDTHH-MM-SSZ>.XML
// Anomaly Report:					<JJJJMMTT>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_ANO_<yyyy-mm-ddThh-mmssZ>.XML
// Confirmation Report:				<JJJJMMTT>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_CNF_<yyyy-mm-ddThh-mmssZ>.XML
// Status Request:					<JJJJMMDD>_SRQ_<EIC-NAME-BILANZKREIS>_<EIC-NAME-ÜNB>.XML
// Schedule format:					<JJJJMMTT>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>.XML

public record FpFileName
{
	private const string XmlFileExtension = ".xml";

	// Gültigkeitsdatum des Fahrplans, bezogen auf den realen Kalendertag [ JJJJMMTT ]
	public string Date { get; init; }

	public string EicNameBilanzkreis { get; init; }

	public string EicNameTso { get; init; }

	public string? Version { get; init; }

	// TPS Trade-responsible Party Schedule BKV-Fahrplan
	// PPS Production-responsible Party Schedule Erzeugerfahrplan
	// SRQ StatusRequest
	public string FahrplanHaendlerTyp { get; init; }

	public FpMessageType MessageType { get; init; }

	// Zeitpunkt der Erstellung der Anomaly bzw. Confirmation Meldung.
	// Der Zeitstempel dient zur Unterscheidung mehrerer Anomaly- (und ggf. auch Confirmation-)
	// Meldungen zu einer Fahrplanmeldung.
	public DateTime? Timestamp { get; init; }

	public static FpFileName FromFileName(string filename)
	{
		if (string.IsNullOrEmpty(filename))
		{
			throw new FormatException("Filename is null or empty.");
		}

		if (!filename.EndsWith(XmlFileExtension, StringComparison.OrdinalIgnoreCase))
		{
			throw new FormatException($"Filename doesnt end with '{XmlFileExtension}'.");
		}

		var coreFilename = filename.Substring(0, filename.LastIndexOf('.'));
		var parts = coreFilename.Split('_');

		if (parts.Length < 4)
		{
			throw new FormatException("Filename does not have the expected format.");
		}

		var date = parts[0];
		var type = parts[1];
		var eicNameBilanzkreis = parts[2];
		var eicNameTso = parts[3];
		var version = "1";
		string? timestamp;
		string messageTypePart;

		if (parts.Length == 4)
		{
			return new FpFileName
			{
				Date = date,
				FahrplanHaendlerTyp = type,
				EicNameBilanzkreis = eicNameBilanzkreis,
				EicNameTso = eicNameTso,
				MessageType = FpMessageType.StatusRequest,
				Timestamp = null,
				Version = version,
			};
		}

		var isNumeric = int.TryParse(parts[4], out _);

		if (isNumeric)
		{
			if (!int.TryParse(parts[4], out int versionInt))
			{
				throw new FormatException("Version number in filename is not numeric");
			}

			version = versionInt.ToString();
			timestamp = parts.Length > 6 ? parts[6] : null;
			messageTypePart = parts[parts.Length - 2];
		}
		else
		{
			version = parts.Length > 5 ? parts[4] : null;
			if (version != null)
			{
				if (!int.TryParse(parts[4], out int versionInt))
				{
					throw new FormatException("Version number in filename is not numeric");
				}

				version = versionInt.ToString();
			}

			timestamp = parts.Length > 5 ? parts[5] : null;
			messageTypePart = parts[parts.Length - 1];
		}

		var messageType = messageTypePart switch
		{
			"ACK" => FpMessageType.Acknowledge,
			"ANO" => FpMessageType.AnomalyReport,
			"CNF" => FpMessageType.ConfirmationReport,
			"CRQ" => FpMessageType.StatusRequest,
			_ => FpMessageType.Schedule
		};


		return new FpFileName
		{
			Date = date,
			FahrplanHaendlerTyp = type,
			EicNameBilanzkreis = eicNameBilanzkreis,
			EicNameTso = eicNameTso,
			MessageType = messageType,
			Timestamp = timestamp == null
				? null
				: DateTime.ParseExact(timestamp, DateTimeFormat.FileTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime(),
			Version = messageType == FpMessageType.StatusRequest ? "1" : version,
		};
	}

	public string ToFileName()
	{
		var datePrefix = GetDatePrefix();

		switch (this.MessageType)
		{
			case FpMessageType.AnomalyReport:
			case FpMessageType.Acknowledge:
			case FpMessageType.ConfirmationReport:
				return
					$"{datePrefix}_{FahrplanHaendlerTyp}_{EicNameBilanzkreis}_{EicNameTso}_{GetVersion()}_{GetMessageTypeValue()}_{GetTimestampPostfix()}{XmlFileExtension}";
			case FpMessageType.Schedule:
				return $"{datePrefix}_{FahrplanHaendlerTyp}_{EicNameBilanzkreis}_{EicNameTso}_{GetVersion()}{XmlFileExtension}";
			case FpMessageType.StatusRequest:
				return $"{datePrefix}_{FahrplanHaendlerTyp}_{EicNameBilanzkreis}_{EicNameTso}{XmlFileExtension}";
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private string GetVersion()
	{
		if (Version == null)
		{
			throw new FormatException("Version is null");
		}
		return Version.PadLeft(3, '0');
	}

	private string GetTimestampPostfix()
	{
		if (Timestamp == null)
		{
			throw new FormatException("MessageDateTime is null");
		}

		return Timestamp.Value.ToFileTimestamp();
	}

	private string GetDatePrefix()
	{
		if (Date == null)
		{
			throw new FormatException("Date is null");
		}

		if (!DateTime.TryParseExact(Date, DateTimeFormat.FileDate, System.Globalization.CultureInfo.InvariantCulture,
			    DateTimeStyles.AssumeUniversal, out DateTime toDate))
		{
			throw new FormatException($"invalid date format '{Date}' [format: '{DateTimeFormat.FileDate}']");
		}

		return Date;
	}

	private string? GetMessageTypeValue()
	{
		string? messageTypeString = MessageType switch
		{
			FpMessageType.Acknowledge => "ACK",
			FpMessageType.AnomalyReport => "ANO",
			FpMessageType.ConfirmationReport => "CNF",
			_ => throw new NotSupportedException("")
		};
		return messageTypeString;
	}
}