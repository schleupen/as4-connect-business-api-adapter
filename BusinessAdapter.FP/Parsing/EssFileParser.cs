namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Text;
using System.Xml.Linq;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class EssFileParser : IFpFileSpecificParser
{
	public FpFile Parse(XDocument document, string filename, string path)
	{
		string xmlData = File.ReadAllText(path);
		byte[] content = Encoding.UTF8.GetBytes(xmlData);
		FpFileName fpFileName = FpFileName.FromFileName(filename);
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var documentNo = ParseDocumentNo(fpFileName.MessageType, document, ns, fpFileName);
		var documentType = ParseDocumentType(document, ns, fpFileName.MessageType);
		var senderIdentification = ParseSenderIdentification(document, ns);
		var senderRole = ParseSenderRole(document, ns);
		var receiverIdentification = ParseReceiverIdentification(document, ns);
		var receiverRole = ParseReceiverRole(document, ns);
		var scheduleTimeInterval = ParseScheduleTimeInterval(document, path, fpFileName, ns);

		return new FpFile(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			content,
			filename,
			path,
			new FpBDEWProperties(
				documentType,
				documentNo,
				scheduleTimeInterval, // TODO use YYYY-MM-DD for FulfillmentData
				senderIdentification,
				senderRole)
		);
	}

	// TODO missing testcases
	public FpPayloadInfo ParsePayload(XDocument document)
	{
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var senderIdentification = ParseSenderIdentification(document, ns);
		var receiverIdentification = ParseReceiverIdentification(document, ns);
		var messageDateTime = ParseMessageDateTime(document, ns);

		return new FpPayloadInfo(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			messageDateTime);
	}

	private static string ParseScheduleTimeInterval(XDocument document, string path, FpFileName fpFileName, XNamespace? ns)
	{
		string scheduleTimeInterval;
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
			throw new ArgumentException($"failed to parse ScheduleTimeInterval.");
		}

		return scheduleTimeInterval;
	}

	private string ParseDocumentType(XDocument document, XNamespace? ns, FpMessageType type)
	{
		if (type == FpMessageType.Acknowledge)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			return "A17";
		}
		else if (type == FpMessageType.Anomaly)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			return "A16";
		}

		return document.Descendants(ns + "MessageType").First().Attribute("v").Value;
	}

	private string ParseDocumentNo(
		FpMessageType type,
		XDocument doc,
		XNamespace? ns,
		FpFileName fpFileName)
	{
		string? result;
		switch (type)
		{
			case FpMessageType.Acknowledge:
				result = doc.Descendants(ns + "ReceivingMessageVersion").First().Attribute("v").Value;
				break;
			case FpMessageType.Schedule:
				result = doc.Descendants(ns + "MessageVersion").First().Attribute("v").Value;
				break;
			case FpMessageType.Confirmation:
				result = doc.Descendants(ns + "ConfirmedMessageVersion").First().Attribute("v").Value;
				break;
			case FpMessageType.Anomaly:
				result = fpFileName.Version;
				break;
			case FpMessageType.Status:
				result = "1";
				break;
			default:
				throw new ArgumentOutOfRangeException($"unkown FpMessageType: {type}");
		}

		if (result == null)
		{
			throw new ArgumentException($"failed to parse DocumentNo from");
		}

		return result;
	}

	private static string ParseSenderIdentification(XDocument document, XNamespace? ns)
	{
		var result = document.Descendants(ns + "SenderIdentification").FirstOrDefault()?.Attribute("v")?.Value;

		if (result == null)
		{
			throw new ArgumentException($"failed to parse SenderIdentification.");
		}

		return result;
	}

	private static string ParseReceiverIdentification(XDocument document, XNamespace? ns)
	{
		var result = document.Descendants(ns + "ReceiverIdentification").FirstOrDefault()?.Attribute("v")?.Value;
		if (result == null)
		{
			throw new ArgumentException($"failed to parse ReceiverIdentification.");
		}

		return result;
	}

	private static string ParseSenderRole(XDocument document, XNamespace? ns)
	{
		var result = document.Descendants(ns + "SenderRole").First().Attribute("v").Value;

		if (result == null)
		{
			throw new ArgumentException($"failed to parse SenderRole.");
		}

		return result;
	}

	private static string? ParseReceiverRole(XDocument document, XNamespace? ns)
	{
		var result = document.Descendants(ns + "ReceiverRole").FirstOrDefault()?.Attribute("v")?.Value;

		if (result == null)
		{
			throw new ArgumentException($"failed to parse ReceiverRole.");
		}

		return result;
	}

	private static DateTime ParseMessageDateTime(XDocument document, XNamespace? ns)
	{
		var result = document.Descendants(ns + "MessageDateTime").FirstOrDefault()?.Attribute("v").Value;
		if (result == null)
		{
			throw new ArgumentException($"failed to parse MessageDateTime.");
		}

		return DateTime.Parse(result).ToUniversalTime();
	}
}