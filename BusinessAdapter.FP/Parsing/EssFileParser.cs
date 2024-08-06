namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Text;
using System.Xml.Linq;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public class EssFileParser : IFpFileSpecificParser
{
	public FpFile Parse(XDocument document, string filename, string path)
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

		FpBDEWProperties properties =
			new FpBDEWProperties(fpFileName.MessageType.ToString(), documentNo, scheduleTimeInterval, senderIdentification, senderRole);

		
		return new FpFile(
			new EIC(senderIdentification),
			 new EIC(receiverIdentification),
			content,
			filename,
			path,
			properties
			);
	}

	public FpParsedPayload ParsePayload(XDocument document)
	{
		XNamespace? ns = document.Root?.GetDefaultNamespace();
		
		var senderIdentification = document.Descendants(ns + "SenderIdentification").FirstOrDefault()?.Attribute("v")?.Value;
		if (senderIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve sender code number from payload.");
		}

		var senderRole = document.Descendants(ns + "SenderRole").First().Attribute("v").Value;
		if (senderRole == null)
		{
			throw new ArgumentException($"Could not retrieve sender role from payload.");
		}

		var receiverIdentification = document.Descendants(ns + "ReceiverIdentification").FirstOrDefault()?.Attribute("v")?.Value;
		if (receiverIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve receiver code number from payload.");
		}

		var receiverRole = document.Descendants(ns + "ReceiverRole").FirstOrDefault()?.Attribute("v")?.Value;
		if (receiverRole == null)
		{
			throw new ArgumentException($"Could not retrieve receiver role from payload.");
		}
		
		var messageDateTime = document.Descendants(ns + "MessageDateTime").FirstOrDefault()?.Attribute("v")?.Value;
		if (messageDateTime == null)
		{
			throw new ArgumentException($"Could not retrieve message date time from payload.");
		}

		// TODO how do we get the scheduletimeinterval for acks and status messages?
		string? scheduleTimeInterval  = document.Descendants(ns + "ScheduleTimeInterval").FirstOrDefault()?.Attribute("v")?.Value;
		if (scheduleTimeInterval == null)
		{
			throw new ArgumentException($"Could not retrieve fulfillment date from payload.");
		}

		return new FpParsedPayload(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			messageDateTime,
			scheduleTimeInterval);
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
				return doc.Descendants(ns + "DocumentIdentification").First().Attribute("v").Value;
			case FpMessageType.Schedule:
				return doc.Descendants(ns + "DocumentVersion").First().Attribute("v").Value;
			case FpMessageType.Confirmation:
				return doc.Descendants(ns + "DocumentIdentification").First().Attribute("v").Value;
			case FpMessageType.Anomaly:
				return fpFileName.Version;
			case FpMessageType.Status:
				return "1";
			default:
				throw new ArgumentOutOfRangeException($"unkown FpMessageType: {type}");
		}
	}
}