namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Text;
using System.Xml.Linq;

public class EssFileParser : IFpFileSpecificParser
{
	public IFpFile Parse(XDocument document, string filename, string path)
	{
		string xmlData = File.ReadAllText(path);
		byte[] content = Encoding.UTF8.GetBytes(xmlData);
		FpFileName fpFileName = FpFileName.Parse(filename);
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var documentNo = ParseEssDocumentNoForMessageType(fpFileName.MessageType, document, ns, fpFileName);
		if (documentNo == null)
		{
			throw new ArgumentException($"Could not document number from file {path}.");
		}

		var documentIdentification = document.Descendants(ns + "DocumentIdentification").First().Attribute("v").Value;

		var senderIdentification = document.Descendants(ns + "SenderIdentification").FirstOrDefault()?.Attribute("v")?.Value;
		if (senderIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
		}
		var senderRole = document.Descendants(ns + "SenderRole").First().Attribute("v").Value;
		if (senderRole == null)
		{
			throw new ArgumentException($"Could not retrieve sender role from file {path}.");
		}
		var receiverIdentification = document.Descendants(ns + "ReceiverIdentification").FirstOrDefault()?.Attribute("v")?.Value;
		if (receiverIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
		}
		var receiverRole = document.Descendants(ns + "ReceiverRole").FirstOrDefault()?.Attribute("v")?.Value;
		if (receiverRole == null)
		{
			throw new ArgumentException($"Could not retrieve receiver role from file {path}.");
		}

		string? scheduleTimeInterval = "";
		// For acknowledge und status messages we take the date from the filename
		if (fpFileName.MessageType == FpMessageType.Acknowledge || fpFileName.MessageType == FpMessageType.Status)
		{
			scheduleTimeInterval = fpFileName.Date;
		}
		else
		{
			scheduleTimeInterval = document.Descendants(ns + "ScheduleTimeInterval").FirstOrDefault()?.Attribute("v")?.Value;
		}

		if (scheduleTimeInterval == null)
		{
			throw new ArgumentException($"Could not retrieve fulfillment date from file {path}.");
		}
		return new FpFile(
			content,
			filename,
			documentNo,
			fpFileName.MessageType.ToString(),
			scheduleTimeInterval,
			senderIdentification,
			senderRole,
			path,
			senderIdentification,
			receiverIdentification,
			receiverRole);
	}

	private string ParseEssDocumentNoForMessageType(
		FpMessageType type,
		XDocument doc,
		XNamespace? ns,
		FpFileName fpFileName)
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
				return fpFileName.Version;
			case FpMessageType.Status:
				return "1"; // Is this correct?
			default:
				throw new ArgumentOutOfRangeException($"unkown FpMessageType: {type}");
		}
	}
}