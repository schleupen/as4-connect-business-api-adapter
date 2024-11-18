namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

// Acknowledgement (Schedule):		<JJJJMMDD>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-ÜNB>_<VVV>_ACK_<YYYYMM-DDTHH-MM-SSZ>.XML
// Acknowledgement (StatusRequest):	<JJJJMMDD>_SRQ_<EIC-NAME-BILANZKREIS>_<EIC-NAME-ÜNB>_ACK_<YYYY-MMDDTHH-MM-SSZ>.XML
// Anomaly Report:					<JJJJMMTT>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_ANO_<yyyy-mm-ddThh-mmssZ>.XML
// Confirmation Report:				<JJJJMMTT>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_CNF_<yyyy-mm-ddThh-mmssZ>.XML
// Status Request:					<JJJJMMDD>_SRQ_<EIC-NAME-BILANZKREIS>_<EIC-NAME-ÜNB>.XML
// Schedule format:					<JJJJMMTT>_TPS_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>.XML

public record FpFileName
{
	private const string XmlFileExtension = ".xml";

	// Gültigkeitsdatum des Fahrplans, bezogen auf den realen Kalendertag
	public string Date { get; init; }

	public string EicNameBilanzkreis { get; init; }

	public string EicNameTso { get; init; }

	public string? Version { get; init; }

	// Typ des Händlerfahrplans (3 Zeichen)
	// Typen:
	//        - TPS Trade-responsible Party Schedule BKV-Fahrplan
	//        - PPS Production-responsible Party Schedule Erzeugerfahrplan
	public string FahrplanHaendlerTyp { get; init; }

	public FpMessageType MessageType { get; init; }

	// Zeitpunkt der Erstellung der Anomaly bzw. Confirmation Meldung.
	// Der Zeitstempel dient zur Unterscheidung mehrerer Anomaly- (und ggf. auch Confirmation-)
	// Meldungen zu einer Fahrplanmeldung.
	public string? Timestamp { get; init; }

	public static FpFileName FromFileName(string filename)
	{
		if (string.IsNullOrEmpty(filename))
		{
			throw new FormatException("Filename does is null or empty.");
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
				MessageType = FpMessageType.Status,
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
			"ANO" => FpMessageType.Anomaly,
			"CNF" => FpMessageType.Confirmation,
			"CRQ" => FpMessageType.Status,
			_ => FpMessageType.Schedule
		};

		return new FpFileName
		{
			Date = date,
			FahrplanHaendlerTyp = type,
			EicNameBilanzkreis = eicNameBilanzkreis,
			EicNameTso = eicNameTso,
			MessageType = messageType,
			Timestamp = timestamp,
			Version = messageType == FpMessageType.Status ? "1" : version,
		};
	}

	public string ToFileName()
	{
		DateTime dateTimeStamp;
		if (Date == null)
		{
			dateTimeStamp = DateTime.UtcNow;
		}
		else
		{
			dateTimeStamp = DateTime.Parse(Date).ToUniversalTime();
		}

		DateTime timeStamp;
		if (Timestamp == null)
		{
			timeStamp = DateTime.UtcNow;
		}
		else
		{
			timeStamp = DateTime.Parse(Timestamp).ToUniversalTime();
		}

		if (this.MessageType == FpMessageType.Schedule)
		{
			return $"{dateTimeStamp:yyyyMMdd}_{FahrplanHaendlerTyp}_{EicNameBilanzkreis}_{EicNameTso}_{Version}{XmlFileExtension}";
		}

		if (this.MessageType == FpMessageType.Status)
		{
			return $"{dateTimeStamp:yyyyMMdd}_{FahrplanHaendlerTyp}_{EicNameBilanzkreis}_{EicNameTso}{XmlFileExtension}";
		}

		var messageTypeString = ToMessageTypeValue();

		return $"{dateTimeStamp:yyyyMMdd}_{FahrplanHaendlerTyp}_{EicNameBilanzkreis}_{EicNameTso}_{Version}_{messageTypeString}_{timeStamp:yyyy-MM-ddTHH\\-mm\\-ssZ}{XmlFileExtension}";
	}

	private string? ToMessageTypeValue()
	{
		string? messageTypeString = MessageType switch
		{
			FpMessageType.Acknowledge => "ACK",
			FpMessageType.Anomaly => "ANO",
			FpMessageType.Confirmation => "CNF",
			_ => throw new NotSupportedException("")
		};
		return messageTypeString;
	}
}