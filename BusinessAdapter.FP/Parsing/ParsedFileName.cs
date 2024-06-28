namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

// ACK format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_ACK_<yyyy-mmddThh-mm-ssZ>.XML
// ANO format: <JJJJMMTT>_<TYP>_<EIC-NAME- BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_ANO_<yyyy-mm-ddThh-mmssZ>.XML
// CON format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_CNF_<yyyy-mm-ddThh-mmssZ>.XML
// Status format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_CRQ.XML
// Schedule format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>.XML

public class ParsedFileName
{
    // Gültigkeitsdatum des Fahrplans, bezogen auf den realen Kalendertag
    public string Date { get; set; }

    public string EicNameBilanzkreis { get; set; }

    public string EicNameTso { get; set; }

    public string Version { get; set; }

    // Typ des Händlerfahrplans (3 Zeichen)
    // Typen:
    //        - TPS Trade-responsible Party Schedule BKV-Fahrplan
    //        - PPS Production-responsible Party Schedule Erzeugerfahrplan
    public string Type { get; set; }

    public FpMessageType MessageType { get; set; }

    // Zeitpunkt der Erstellung der Anomaly bzw. Confirmation Meldung.
    // Der Zeitstempel dient zur Unterscheidung mehrerer Anomaly- (und ggf. auch Confirmation-)
    // Meldungen zu einer Fahrplanmeldung.
    public string Timestamp { get; set; }

    public static ParsedFileName Parse(string filename)
    {
        var coreFilename = filename.Substring(0, filename.LastIndexOf('.'));
        var parts = coreFilename.Split('_');

        if (parts.Length < 5)
        {
            throw new FormatException("Filename does not have the expected format.");
        }

        ParsedFileName parsedFilename = new ParsedFileName
        {
            Date = parts[0],
            Type = parts[1],
            EicNameBilanzkreis = parts[2],
            EicNameTso = parts[3]
        };

        var isNumeric = int.TryParse(parts[4], out _);
        string messageTypePart = "";

        if (isNumeric)
        {
            parsedFilename.Version = parts[4];
            parsedFilename.Timestamp = parts.Length > 6 ? parts[6] : null;
            messageTypePart = parts[^2];
        }
        else
        {
            parsedFilename.Version = parts.Length > 5 ? parts[4] : null;
            parsedFilename.Timestamp = parts.Length > 5 ? parts[5] : null;
            messageTypePart = parts[^1];
        }

        parsedFilename.MessageType = messageTypePart switch
        {
            "ACK" => FpMessageType.Acknowledge,
            "ANO" => FpMessageType.Anomaly,
            "CNF" => FpMessageType.Confirmation,
            "CRQ" => FpMessageType.Status,
            _ => FpMessageType.Schedule
        };

        return parsedFilename;
    }

    public override string ToString()
    {
        return $"Date: {Date}, Type: {Type}, EicNameBilanzkreis: {EicNameBilanzkreis}, EicNameTso: {EicNameTso}, Version: {Version}, Timestamp: {Timestamp}, MessageType: {MessageType}";
    }

    public string GenerateFilename()
    {
        string messageTypeString = MessageType switch
        {
            FpMessageType.Acknowledge => "ACK",
            FpMessageType.Anomaly => "ANO",
            FpMessageType.Confirmation => "CNF",
            FpMessageType.Status => "CRQ",
            _ => null
        };

        if (messageTypeString == null)
        {
            return $"{Date}_{Type}_{EicNameBilanzkreis}_{EicNameTso}_{Version}.XML";
        }

        if (!string.IsNullOrEmpty(Timestamp))
        {
            return $"{Date}_{Type}_{EicNameBilanzkreis}_{EicNameTso}_{Version}_{messageTypeString}_{Timestamp}.XML";
        }
        else
        {
            return $"{Date}_{Type}_{EicNameBilanzkreis}_{EicNameTso}_{messageTypeString}.XML";
        }
    }
}