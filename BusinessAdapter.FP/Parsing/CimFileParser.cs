namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class CimFileParser : IFpFileSpecificParser
{
	public FpFile Parse(XDocument document, string filename, string path)
	{
		string xmlData = File.ReadAllText(path);
		byte[] content = Encoding.UTF8.GetBytes(xmlData);
		FpFileName fpFileName = FpFileName.FromFileName(filename);
		var messageType = fpFileName.MessageType;
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var documentNo = ParseCimDocumentNoForMessageType(fpFileName.MessageType, document, ns, fpFileName);
		if (documentNo == null)
		{
			throw new ArgumentException($"Could not document number from file {path}.");
		}

		var documentType = "";
		if (fpFileName.MessageType == FpMessageType.Acknowledge)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			documentType = BDEWDocumentTypes.A17;
		}
		else if (fpFileName.MessageType == FpMessageType.AnomalyReport)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			documentType = BDEWDocumentTypes.A16;
		}
		else
		{
			documentType = document.Descendants(ns + "type").First().Value;
		}

		var senderIdentification = document.Descendants(ns + "sender_MarketParticipant.mRID").FirstOrDefault()?.Value;
		if (senderIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
		}

		var senderRole = document.Descendants(ns + "sender_MarketParticipant.marketRole.type").FirstOrDefault()?.Value;
		if (senderRole == null)
		{
			throw new ArgumentException($"Could not retrieve sender role from file {path}.");
		}

		var receiverIdentification = document.Descendants(ns + "receiver_MarketParticipant.mRID").FirstOrDefault()?.Value;
		if (receiverIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
		}

		var receiverRole = document.Descendants(ns + "receiver_MarketParticipant.marketRole.type").FirstOrDefault()?.Value;
		if (receiverRole == null)
		{
			throw new ArgumentException($"Could not retrieve receiver role from file {path}.");
		}

		var bdewFulfillmentDate = ParseBDEWFulfillmentDate(document, messageType, fpFileName.Date, ns);

		FpBDEWProperties bdewProperties = new FpBDEWProperties(
			documentType,
			documentNo,
			bdewFulfillmentDate,
			senderIdentification,
			senderRole);

		return new FpFile(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			content,
			filename,
			path,
			bdewProperties);
	}

	private static string ParseBDEWFulfillmentDate(XDocument document, FpMessageType messageType, string fileDate, XNamespace? ns)
	{
		switch (messageType)
		{
			case FpMessageType.Acknowledge:
			case FpMessageType.StatusRequest:
				if (DateTime.TryParseExact(fileDate, DateTimeFormat.FileDate, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime fileDateParsed))
				{
					return fileDateParsed.ToHyphenDate();
				}

				break;
			case FpMessageType.ConfirmationReport:
				var cnfTimeInterval = ParseTimeInterval(document, ns, "schedule_Period.timeInterval");
				return cnfTimeInterval.End.ToHyphenDate();
			case FpMessageType.Schedule:
			case FpMessageType.AnomalyReport:
				var timeInterval = ParseTimeInterval(document, ns, "schedule_Time_Period.timeInterval");
				return timeInterval.End.ToHyphenDate();
			default:
				throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
		}


		throw new ValidationException("could not parse BDEWFulfillmentDate");
	}

	private static (DateTime Start, DateTime End) ParseTimeInterval(XDocument document, XNamespace? ns, string elementName)
	{
		var timeInterval = document.Descendants(ns + elementName).FirstOrDefault();
		if (timeInterval == null)
		{
			throw new ValidationException($"Element '{elementName}' not found");
		}

		var startTimeInterval = timeInterval.Descendants(ns + "start").FirstOrDefault()?.Value;
		var endTimeInterval = timeInterval.Descendants(ns + "end").FirstOrDefault()?.Value;

		if (startTimeInterval is null)
		{
			throw new ValidationException($"Element '{elementName}.start' not found");
		}
		if (endTimeInterval is null)
		{
			throw new ValidationException($"Element '{elementName}.end' not found");
		}

		if (!DateTime.TryParse(startTimeInterval, out DateTime startDate))
		{
			throw new ValidationException($"could not parse value '{startTimeInterval}' from '{elementName}.start' as date.");
		}

		if (!DateTime.TryParse(endTimeInterval, out DateTime endDate))
		{
			throw new ValidationException($"could not parse value '{startTimeInterval}' from '{elementName}.end' as state'");
		}

		return (startDate, endDate);
	}

	private static string ParseElementValueOrThrow(XDocument document, XNamespace? ns, string elementName)
	{
		// TODO better use XPath?
		var value = document.Descendants(ns + elementName).SingleOrDefault()?.Attribute("v")?.Value;

		if (value == null)
		{
			throw new ValidationException($"failed to parse '{elementName}'.");
		}

		return value;
	}

	public FpPayloadInfo ParsePayload(XDocument document)
	{
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var senderIdentification = document.Descendants(ns + "sender_MarketParticipant.mRID").FirstOrDefault()?.Value;
		if (senderIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve sender code number from Payload.");
		}

		var senderRole = document.Descendants(ns + "sender_MarketParticipant.marketRole.type").FirstOrDefault()?.Value;
		if (senderRole == null)
		{
			throw new ArgumentException($"Could not retrieve sender role from ffrom Payload.");
		}

		var receiverIdentification = document.Descendants(ns + "receiver_MarketParticipant.mRID").FirstOrDefault()?.Value;
		if (receiverIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve receiver code number from Payload.");
		}

		var receiverRole = document.Descendants(ns + "receiver_MarketParticipant.marketRole.type").FirstOrDefault()?.Value;
		if (receiverRole == null)
		{
			throw new ArgumentException($"Could not retrieve receiver role from Payload.");
		}

		string? scheduleTimeInterval = "";

		// For acknowledge und status messages we take the date from the filename
		var startTimeInterval = document.Descendants(ns + "sender_MarketParticipant.start").FirstOrDefault()?.Value;
		var endTimeInterval = document.Descendants(ns + "sender_MarketParticipant.end").FirstOrDefault()?.Value;

		if (startTimeInterval is not null && endTimeInterval is not null)
		{
			scheduleTimeInterval = startTimeInterval + "/" + endTimeInterval;
		}

		var creationDateTime = ParseCreationDateTime(document, ns);

		if (scheduleTimeInterval == null)
		{
			throw new ArgumentException($"Could not retrieve fulfillment date from Payload.");
		}

		return new FpPayloadInfo(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			creationDateTime,
			FpMessageType.Acknowledge, // TODO
			"TPS"); // TODO
	}

	private static DateTime ParseCreationDateTime(XDocument document, XNamespace? ns)
	{
		var value = document.Descendants(ns + "createdDateTime").FirstOrDefault()?.Value;

		return DateTime.Parse(value);
	}

	private static string? ParseCimDocumentNoForMessageType(
		FpMessageType type,
		XDocument doc,
		XNamespace? ns,
		FpFileName fpFileName)
	{
		switch (type)
		{
			case FpMessageType.Acknowledge:
				return doc.Descendants(ns + "received_MarketDocument.revisionNumber").First().Value;
			case FpMessageType.Schedule:
				return doc.Descendants(ns + "revisionNumber").First().Value;
			case FpMessageType.ConfirmationReport:
				return doc.Descendants(ns + "confirmed_MarketDocument.revisionNumber").First().Value;
			case FpMessageType.AnomalyReport:
				return fpFileName.Version;
			case FpMessageType.StatusRequest:
				return "1"; // Is this correct?
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}