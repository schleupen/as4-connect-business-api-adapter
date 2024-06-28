namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FileNameExtractor : IFileNameExtractor
{
    public string ExtractFilename(InboxFpMessage mpMessage)
    {
        ParsedFileName parsedFileName = new ParsedFileName();
        parsedFileName.MessageType = ToMessageType(mpMessage.BDEWDocumentType);
        parsedFileName.EicNameBilanzkreis = ""; // TODO Mapping
        parsedFileName.EicNameTso = ""; // TODO Mapping
        parsedFileName.Timestamp = mpMessage.BDEWFulfillmentDate; // TODO is this correct?
        parsedFileName.Date = mpMessage.BDEWFulfillmentDate; // TODO is this correct?
        parsedFileName.Version = ""; // TODO parse?
        parsedFileName.Type = ""; // TODO parse?

        return parsedFileName.GenerateFilename();
    }
    
    private FpMessageType ToMessageType(string bdewDocumentType)
    {
        // TODO
        return FpMessageType.Acknowledge;
    }
}   