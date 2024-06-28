namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Text;
using System.Xml.Linq;

public class FpFileParser : IFpFileParser
{
    private readonly IFileSystemWrapper fileSystemWrapper;
    
    private readonly string ESS_NAMESPACE_STRING = "urn:entsoe.eu:wgedi:ess";
    
    public FpFileParser(IFileSystemWrapper fileSystemWrapper)
    {
        this.fileSystemWrapper = fileSystemWrapper;
    }

    public IFpFile Parse(string path)
    {
         string filename = fileSystemWrapper.GetFileName(path);
         
         XDocument doc = XDocument.Load(path);

         XNamespace? ns = doc.Root?.GetDefaultNamespace();

         string xmlData = File.ReadAllText(path);

         byte[] content = Encoding.UTF8.GetBytes(xmlData);
         
         ParsedFileName parsedFileName = ParsedFileName.Parse(filename);
         
         // TODO maybe find a better way to determine the format
         if (ns!.NamespaceName.Contains(ESS_NAMESPACE_STRING))
         {
             return ParseEssFile(parsedFileName, doc, filename, path, content);
         }
         else
         {
             return ParseCimFile(parsedFileName, doc, filename, path, content);
         }
    }

    private IFpFile ParseEssFile(ParsedFileName parsedFileName,
        XDocument doc, 
        string filename,
        string path,
        byte[] content)
    {
        XNamespace? ns = doc.Root?.GetDefaultNamespace();

        var documentNo = ParseESSDocumentNoForMessageType(parsedFileName.MessageType, doc, ns, parsedFileName);
        if (documentNo == null)
        {
            throw new ArgumentException($"Could not document number from file {path}.");
        }
   
        var documentIdentification = doc.Descendants(ns + "DocumentIdentification").First().Attribute("v").Value;
    
        var senderIdentification = doc.Descendants(ns + "SenderIdentification").First().Attribute("v").Value;
        if (senderIdentification == null)
        {
            throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
        }
        var senderRole = doc.Descendants(ns + "SenderRole").First().Attribute("v").Value;
        if (senderRole == null)
        {
            throw new ArgumentException($"Could not retrieve sender role from file {path}.");
        }
        var receiverIdentification = doc.Descendants(ns + "ReceiverIdentification").First().Attribute("v").Value;
        if (receiverIdentification == null)
        {
            throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
        }
        var receiverRole = doc.Descendants(ns + "ReceiverRole").First().Attribute("v").Value;
        if (receiverRole == null)
        {
            throw new ArgumentException($"Could not retrieve receiver role from file {path}.");
        }
        
        string? scheduleTimeInterval = "";
        // For acknowledge und status messages we take the date from the filename
        if (parsedFileName.MessageType == FpMessageType.Acknowledge || parsedFileName.MessageType == FpMessageType.Status)
        {
            scheduleTimeInterval = parsedFileName.Date;
        }
        else
        {
            scheduleTimeInterval = doc.Descendants(ns + "ScheduleTimeInterval").First().Attribute("v").Value;
        }
        
        if (scheduleTimeInterval == null)
        {
            throw new ArgumentException($"Could not retrieve fulfillment date from file {path}.");
        }
        return new FpFile(
            content,
            filename,
            documentNo,
            parsedFileName.MessageType.ToString(),
            scheduleTimeInterval,
            senderIdentification,
            senderRole,
            path,
            senderIdentification,
            receiverIdentification,
            receiverRole);
    }

    private IFpFile ParseCimFile(ParsedFileName parsedFileName,
        XDocument doc,
        string filename,
        string path,
        byte[] content)
    {
        XNamespace? ns = doc.Root?.GetDefaultNamespace();

        var documentNo = ParseCIMDocumentNoForMessageType(parsedFileName.MessageType, doc, ns, parsedFileName);
        if (documentNo == null)
        {
            throw new ArgumentException($"Could not document number from file {path}.");
        }
        
        var documentIdentification = doc.Descendants(ns + "mRID").First().Attribute("v").Value;
        
        var senderIdentification = doc.Descendants(ns + "sender_MarketParticipant.mRID").First().Attribute("v").Value;
        if (senderIdentification == null)
        {
            throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
        }
        var senderRole = doc.Descendants(ns + "sender_MarketParticipant.marketRole.type").First().Attribute("v").Value;
        if (senderRole == null)
        {
            throw new ArgumentException($"Could not retrieve sender role from file {path}.");
        }
        var receiverIdentification = doc.Descendants(ns + "sender_MarketParticipant.mRID").First().Attribute("v").Value;
        if (receiverIdentification == null)
        {
            throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
        }
        var receiverRole = doc.Descendants(ns + "sender_MarketParticipant.marketRole.type").First().Attribute("v").Value;
        if (receiverRole == null)
        {
            throw new ArgumentException($"Could not retrieve receiver role from file {path}.");
        }
        
        string? scheduleTimeInterval = "";
        // For acknowledge und status messages we take the date from the filename
        if (parsedFileName.MessageType == FpMessageType.Acknowledge || parsedFileName.MessageType == FpMessageType.Status)
        {
            scheduleTimeInterval = parsedFileName.Date;
        }
        else
        {
            var startTimeInterval = doc.Descendants(ns + "sender_MarketParticipant.start").First().Attribute("v")?.Value;
            var endTimeInterval = doc.Descendants(ns + "sender_MarketParticipant.end").First().Attribute("v")?.Value;

            scheduleTimeInterval = startTimeInterval + "/" + endTimeInterval;
        }
        
        if (scheduleTimeInterval == null)
        {
            throw new ArgumentException($"Could not retrieve fulfillment date from file {path}.");
        }
        
        return new FpFile( 
            content,
            filename,
            documentNo,
            parsedFileName.MessageType.ToString(),
            scheduleTimeInterval,
            senderIdentification,
            senderRole,
            path,
            senderIdentification,
            receiverIdentification,
            receiverRole);
    }

    private string ParseESSDocumentNoForMessageType(
        FpMessageType type, 
        XDocument doc,
        XNamespace? ns,
        ParsedFileName parsedFileName)
    {
        switch (type)
        {
            case FpMessageType.Acknowledge:
                // TODO not sure if this is correct
                return doc.Descendants(ns + "DocumentIdentification").First().Attribute("v").Value;
            case FpMessageType.Schedule:
                return doc.Descendants(ns + "DocumentVersion").First().Attribute("v").Value;
            case FpMessageType.Confirmation:
                return doc.Descendants(ns + "DocumentIdentification").First().Attribute("v").Value;
            case FpMessageType.Anomaly:
                return parsedFileName.Version;
            case FpMessageType.Status:
                return "1"; // Is this correct?
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private string? ParseCIMDocumentNoForMessageType(
        FpMessageType type, 
        XDocument doc,
        XNamespace? ns,
        ParsedFileName parsedFileName)
    {
        switch (type)
        {
            case FpMessageType.Acknowledge:
                // This should be received_market-document.revisionNumber
                return doc.Descendants(ns + "revisionNumber").First().Attribute("v").Value;
            case FpMessageType.Schedule:
                return doc.Descendants(ns + "revisionNumber").First().Attribute("v").Value;
            case FpMessageType.Confirmation:
                // This should be confirmed_market-document.revisionNumber
                return doc.Descendants(ns + "revisionNumber").First().Attribute("v").Value;
            case FpMessageType.Anomaly:
                return parsedFileName.Version;
            case FpMessageType.Status:
                return "1"; // Is this correct?
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}