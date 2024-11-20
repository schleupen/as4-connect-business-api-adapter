namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

		var messageType = fpFileName.MessageType;

		var documentNo = ParseDocumentNo(messageType, document, ns, fpFileName);
		var documentType = ParseDocumentType(messageType, document, ns);
		var senderIdentification = ParseSenderIdentification(document, ns);
		var senderRole = ParseSenderRole(document, ns);
		var receiverIdentification = ParseReceiverIdentification(document, ns);
		var fulfillmentDate = ParseFulfillmentDate(messageType, document, fpFileName.Date, ns);

		return new FpFile(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			content,
			filename,
			path,
			new FpBDEWProperties(
				documentType,
				documentNo,
				fulfillmentDate,
				senderIdentification,
				senderRole)
		);
	}

	// TODO missing testcases
	public FpPayloadInfo ParsePayload(XDocument document)
	{
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var messageType = ParseMessageType(document, ns);

		return new FpPayloadInfo(
			new EIC(ParseSenderIdentification(document, ns)),
			new EIC(ParseReceiverIdentification(document, ns)),
			ParseMessageDateTime(document, ns),
			messageType,
			GetFahrplanhaendlerTyp(messageType, document, ns));
	}

	private string GetFahrplanhaendlerTyp(FpMessageType messageType, XDocument doc, XNamespace? ns)
	{
		switch (messageType)
		{
			case FpMessageType.Acknowledge:
				return "TPS"; // this is only correct for acknowledge messages received in response to a schedule message. To identify the correct types (TPS, SRQ) we need to establish persistence for outgoing messages
			case FpMessageType.AnomalyReport:
			case FpMessageType.ConfirmationReport:
			case FpMessageType.Schedule:
				return "TPS";
			case FpMessageType.StatusRequest:
				return "SRQ";
			default:
				throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
		}
	}

	private FpMessageType ParseMessageType(XDocument document, XNamespace? ns)
	{
		var rootName = document.Root?.Name.LocalName;

		switch (rootName)
		{
			case "AnomalyReport":
				return FpMessageType.AnomalyReport;
			case "StatusRequest":
				return FpMessageType.StatusRequest;
			case "ConfirmationReport":
				return FpMessageType.ConfirmationReport;
			case "ScheduleMessage":
				return FpMessageType.Schedule;
			case "AcknowledgementMessage":
				return FpMessageType.Acknowledge;
			default:
				throw new ValidationException($"failed to parse MessageType (XmlRoot: '{rootName}').");
		}
	}

	private static string ParseFulfillmentDate(FpMessageType messageType, XDocument document, string dateFromFileName, XNamespace? ns)
	{
		// For acknowledge und status messages we take the date from the filename
		if (messageType == FpMessageType.Acknowledge || messageType == FpMessageType.StatusRequest)
		{
			if (DateTime.TryParseExact(dateFromFileName, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture,
				    DateTimeStyles.AssumeUniversal, out DateTime toDate))
			{
				return toDate.ToUniversalTime().ToString("yyyy-MM-dd");
			}
		}
		else
		{
			var scheduleTimeInterval = ParseElementValueOrThrow(document, ns, "ScheduleTimeInterval");

			var parts = scheduleTimeInterval.Split('/');
			if (parts.Length == 2)
			{
				var to = parts[1];

				if (DateTime.TryParse(to, out DateTime toDate))
				{
					return toDate.ToUniversalTime().ToString("yyyy-MM-dd");
				}
			}
		}

		throw new ValidationException($"failed to parse ScheduleTimeInterval.");
	}

	private string ParseDocumentType(FpMessageType type, XDocument doc, XNamespace? ns)
	{
		if (type == FpMessageType.Acknowledge)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			return BDEWDocumentTypes.A17;
		}
		else if (type == FpMessageType.AnomalyReport)
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			return BDEWDocumentTypes.A16;
		}

		return ParseElementValueOrThrow(doc, ns, "MessageType");
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
				result = ParseElementValueOrThrow(doc, ns, "ReceivingMessageVersion");
				break;
			case FpMessageType.Schedule:
				result = ParseElementValueOrThrow(doc, ns, "MessageVersion");
				break;
			case FpMessageType.ConfirmationReport:
				result = ParseElementValueOrThrow(doc, ns, "ConfirmedMessageVersion");
				break;
			case FpMessageType.AnomalyReport:
				result = fpFileName.Version;
				break;
			case FpMessageType.StatusRequest:
				result = "1";
				break;
			default:
				throw new ArgumentOutOfRangeException($"unkown FpMessageType: {type}");
		}

		if (result == null)
		{
			throw new ValidationException($"failed to parse DocumentNo from");
		}

		return result;
	}

	private static string ParseSenderIdentification(XDocument document, XNamespace? ns)
	{
		return ParseElementValueOrThrow(document, ns, "SenderIdentification");
	}

	private static string ParseReceiverIdentification(XDocument document, XNamespace? ns)
	{
		return ParseElementValueOrThrow(document, ns, "ReceiverIdentification");
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

	private static string ParseSenderRole(XDocument document, XNamespace? ns)
	{
		return ParseElementValueOrThrow(document, ns, "SenderRole");
	}

	private static DateTime ParseMessageDateTime(XDocument document, XNamespace? ns)
	{
		var value = ParseElementValueOrThrow(document, ns, "MessageDateTime");

		return DateTime.Parse(value).ToUniversalTime();
	}
}