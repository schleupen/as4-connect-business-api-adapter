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

		var documentNo = ParseEssDocumentNoForMessageType(fpFileName.MessageType, document, ns, fpFileName);
		if (documentNo == null)
		{
			throw new ArgumentException($"Could not document number from file {path}.");
		}

		var documentType = "";
		if (fpFileName.MessageType == FpMessageType.Acknowledge)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			documentType = "A17";
		}
		else if (fpFileName.MessageType == FpMessageType.Anomaly)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			documentType = "A16";
		}
		else
		{
			documentType = document.Descendants(ns + "MessageType").First().Attribute("v").Value;
		}


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
			new FpBDEWProperties(documentType, documentNo, scheduleTimeInterval, senderIdentification, senderRole);


		return new FpFile(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			content,
			filename,
			path,
			properties
		);
	}

	// TODO missing testcases
	public FpPayloadInfo ParsePayload(XDocument document)
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
			throw new ArgumentException($"Could not retrieve sender role from paylod.");
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

		string? messageDateTime = document.Descendants(ns + "MessageDateTime").FirstOrDefault()?.Attribute("v").Value;

		return new FpPayloadInfo(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			messageDateTime,
			messageDateTime);
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
				return doc.Descendants(ns + "ReceivingMessageVersion").First().Attribute("v").Value;
			case FpMessageType.Schedule:
				return doc.Descendants(ns + "MessageVersion").First().Attribute("v").Value;
			case FpMessageType.Confirmation:
				return doc.Descendants(ns + "ConfirmedMessageVersion").First().Attribute("v").Value;
			case FpMessageType.Anomaly:
				return fpFileName.Version;
			case FpMessageType.Status:
				return "1";
			default:
				throw new ArgumentOutOfRangeException($"unkown FpMessageType: {type}");
		}
	}
}