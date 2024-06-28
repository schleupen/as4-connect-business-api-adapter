namespace Schleupen.AS4.BusinessAdapter.FP;

// ACK format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_ACK_<yyyy-mmddThh-mm- ssZ>.XML
// ANO format: <JJJJMMTT>_<TYP>_<EIC-NAME- BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_ANO_<yyyy-mm-ddThh-mmssZ>.XML 
// CON format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>_CNF_<yyyy-mm-ddThh-mmssZ>.XML 
// Status format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_CRQ.XML 
// Schedule format: <JJJJMMTT>_<TYP>_<EIC-NAME-BILANZKREIS>_<EIC-NAME-TSO>_<VVV>.XML

//20240126_CNF_0X1001A1001A264_FINGRID_002_CNF_01-26T08-2344Z.XML
    
//20240125_ANO_0X1001A1001A264_FINGRID_002_ANO_2024-01-26T08-2344Z

// 20240125_PPS_0X1001A1001A264_FINGRID_002.XML
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
}