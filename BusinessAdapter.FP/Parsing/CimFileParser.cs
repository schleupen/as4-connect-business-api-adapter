namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class CimFileParser : IFpFileSpecificParser
{
	public const string XmlNamespace = "urn:iec62325.351:tc57wg16:451";
	private const string XmlElementNameSender = "sender_MarketParticipant.mRID";
	private const string XmlElementNameSenderRole = "sender_MarketParticipant.marketRole.type";
	private const string XmlElementNameReceiver = "receiver_MarketParticipant.mRID";
	private const string XmlElementNameSchedulePeriod = "schedule_Period.timeInterval";
	private const string XmlElementNameScheduleTimePeriod = "schedule_Time_Period.timeInterval";
	private const string XmlElementNameStart = "start";
	private const string XmlElementNameEnd = "end";
	private const string XmlElementNameCreated = "createdDateTime";
	private const string XmlElementNameAcknowledgementRevisionNumber = "received_MarketDocument.revisionNumber";
	private const string XmlElementNameScheduleRevisionNumber = "revisionNumber";
	private const string XmlElementNameConfirmationRevisionNumber = "confirmed_MarketDocument.revisionNumber";

	public FpFile Parse(XDocument document, string filename, string path)
	{
		var root = document.Root ?? throw new ValidationException("missing root element");

		FpFileName fpFileName = FpFileName.FromFileName(filename);
		var messageType = fpFileName.MessageType;
		XNamespace? ns = root.GetDefaultNamespace();

		var senderIdentification = ParseSenderIdentification(root, ns);
		var receiverIdentification = ParseReceiverIdentification(root, ns);
		var documentNo = ParseDocumentNo(root, ns, messageType, fpFileName.Version);
		var documentType = ParseDocumentType(root, ns, messageType);
		var senderRole = ParseSenderRole(root, ns);
		var bdewFulfillmentDate = ParseBDEWFulfillmentDate(root, messageType, fpFileName.Date, ns);

		return new FpFile(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			Encoding.UTF8.GetBytes(File.ReadAllText(path)),
			filename,
			path,
			new FpBDEWProperties(
				documentType,
				documentNo,
				bdewFulfillmentDate,
				senderIdentification,
				senderRole));
	}

	public FpPayloadInfo ParsePayload(XDocument document)
	{
		XNamespace? ns = document.Root?.GetDefaultNamespace();
		XElement root = document.Root ?? throw new ValidationException("missing root element");

		return new FpPayloadInfo(
			new EIC(ParseSenderIdentification(root, ns)),
			new EIC(ParseReceiverIdentification(root, ns)),
			ParseCreationDateTime(root, ns),
			ParseMessageType(document),
			GetFahrplanhaendlerTyp(ParseMessageType(document)));
	}

	private static string ParseDocumentNo(XElement root, XNamespace? ns, FpMessageType messageType, string? versionFromFile)
	{
		return messageType switch
		{
			FpMessageType.Acknowledge => root.Element(ns + XmlElementNameAcknowledgementRevisionNumber)?.Value ?? throw new ValidationException($"missing xml element '{XmlElementNameAcknowledgementRevisionNumber}'"),
			FpMessageType.Schedule => root.Element(ns + XmlElementNameScheduleRevisionNumber)?.Value ?? throw new ValidationException($"missing xml element '{XmlElementNameScheduleRevisionNumber}'"),
			FpMessageType.ConfirmationReport => root.Element(ns + XmlElementNameConfirmationRevisionNumber)?.Value ?? throw new ValidationException($"missing xml element '{XmlElementNameConfirmationRevisionNumber}'"),
			FpMessageType.AnomalyReport => versionFromFile ?? throw new ValidationException("missing version"),
			FpMessageType.StatusRequest => "1", // Is this correct?
			_ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
		};
	}

	private static string ParseDocumentType(XElement element, XNamespace? ns, FpMessageType messageType)
	{
		return messageType switch
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			FpMessageType.Acknowledge => BDEWDocumentTypes.A17,
			FpMessageType.AnomalyReport => BDEWDocumentTypes.A16,
			FpMessageType.Schedule or FpMessageType.ConfirmationReport or FpMessageType.StatusRequest => element.Element(ns + "type")?.Value ?? throw new ValidationException("missing xml element 'type'"),
			_ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
		};
	}

	private static string ParseSenderIdentification(XElement root, XNamespace? ns)
	{
		return root.Element(ns + XmlElementNameSender)?.Value
		       ?? throw new ValidationException($"missing xml element {XmlElementNameSender}");
	}

	private static string ParseSenderRole(XElement root, XNamespace? ns)
	{
		return root.Element(ns + XmlElementNameSenderRole)?.Value
		       ?? throw new ValidationException($"missing xml element {XmlElementNameSenderRole}");
	}

	private static string ParseReceiverIdentification(XElement root, XNamespace? ns)
	{
		return root.Element(ns + XmlElementNameReceiver)?.Value
		       ?? throw new ValidationException($"missing xml element {XmlElementNameReceiver}");
	}

	private static string ParseBDEWFulfillmentDate(XElement root, FpMessageType messageType, string fileDate, XNamespace? ns)
	{
		switch (messageType)
		{
			case FpMessageType.Acknowledge:
			case FpMessageType.StatusRequest:
				if (DateTime.TryParseExact(fileDate,
					    DateTimeFormat.FileDate,
					    System.Globalization.CultureInfo.InvariantCulture,
					    DateTimeStyles.AssumeUniversal,
					    out DateTime fileDateParsed))
				{
					return fileDateParsed.ToHyphenDate();
				}
				break;
			case FpMessageType.ConfirmationReport:
				var cnfTimeInterval = ParseTimeInterval(root, ns, XmlElementNameSchedulePeriod);
				return cnfTimeInterval.End.ToHyphenDate();
			case FpMessageType.Schedule:
			case FpMessageType.AnomalyReport:
				var timeInterval = ParseTimeInterval(root, ns, XmlElementNameScheduleTimePeriod);
				return timeInterval.End.ToHyphenDate();
			default:
				throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
		}

		throw new ValidationException("could not parse BDEWFulfillmentDate");
	}

	private static (DateTime Start, DateTime End) ParseTimeInterval(XElement root, XNamespace? ns, string elementName)
	{
		var startTimeInterval = root.Element(ns + elementName)?.Element(ns + XmlElementNameStart)?.Value;
		var endTimeInterval = root.Element(ns + elementName)?.Element(ns + XmlElementNameEnd)?.Value;

		if (startTimeInterval is null)
		{
			throw new ValidationException($"missing xml element '{elementName}.{XmlElementNameStart}'");
		}

		if (endTimeInterval is null)
		{
			throw new ValidationException($"missing xml element '{elementName}.{XmlElementNameEnd}'");
		}

		if (!DateTime.TryParse(startTimeInterval, out DateTime startDate))
		{
			throw new ValidationException($"invalid date format '{startTimeInterval}'.");
		}

		if (!DateTime.TryParse(endTimeInterval, out DateTime endDate))
		{
			throw new ValidationException($"invalid date format '{endTimeInterval}'.");
		}

		return (startDate, endDate);
	}

	private string GetFahrplanhaendlerTyp(FpMessageType messageType)
	{
		return messageType switch
		{
			FpMessageType.Acknowledge => "TPS", // this is only correct for acknowledge messages received in response to a schedule message. To identify the correct types (TPS, SRQ) we need to establish persistence for outgoing messages
			FpMessageType.AnomalyReport or FpMessageType.ConfirmationReport or FpMessageType.Schedule => "TPS",
			FpMessageType.StatusRequest => "SRQ",
			_ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
		};
	}

	private static DateTime ParseCreationDateTime(XElement document, XNamespace? ns)
	{
		var creationDate = document.Element(ns + XmlElementNameCreated)?.Value ?? throw new ValidationException($"missing xml element '{XmlElementNameCreated}'");
		return DateTime.Parse(creationDate).ToUniversalTime();
	}

	private FpMessageType ParseMessageType(XDocument document)
	{
		var rootName = document.Root?.Name.LocalName;

		return rootName switch
		{
			"AnomalyReport_MarketDocument" => FpMessageType.AnomalyReport,
			"StatusRequest_MarketDocument" => FpMessageType.StatusRequest,
			"Confirmation_MarketDocument" => FpMessageType.ConfirmationReport,
			"Schedule_MarketDocument" => FpMessageType.Schedule,
			"Acknowledgement_MarketDocument" => FpMessageType.Acknowledge,
			_ => throw new ValidationException($"unsupported XmlRoot '{rootName}'.")
		};
	}
}