namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class EssFileParser : IFpFileSpecificParser
{
	public const string XmlNamespace = "urn:entsoe.eu:wgedi:ess";
	private const string XmlElementNameSenderRole = "SenderRole";
	private const string XmlElementNameMessageDateTime = "MessageDateTime";
	private const string XmlElementNameReceiverIdentification = "ReceiverIdentification";
	private const string XmlElementNameSenderIdentification = "SenderIdentification";
	private const string XmlElementNameReceivingMessageVersion = "ReceivingMessageVersion";
	private const string XmlElementNameMessageVersion = "MessageVersion";
	private const string XmlElementNameConfirmedMessageVersion = "ConfirmedMessageVersion";
	private const string XmlElementNameMessageType = "MessageType";
	private const string XmlElementNameScheduleTimeInterval = "ScheduleTimeInterval";

	public FpFile Parse(XDocument document, string filename, string path)
	{
		var root = document.Root ?? throw new ValidationException("missing root element");
		var ns = root.GetDefaultNamespace();

		var fpFileName = FpFileName.FromFileName(filename);
		var messageType = fpFileName.MessageType;

		var documentNo = ParseDocumentNo(root, ns, messageType, fpFileName);
		var documentType = ParseDocumentType(root, ns, messageType);
		var senderIdentification = ParseSenderIdentification(root, ns);
		var senderRole = ParseSenderRole(root, ns);
		var receiverIdentification = ParseReceiverIdentification(root, ns);
		var fulfillmentDate = ParseBDEWFulfillmentDate(root, ns, messageType, fpFileName.Date);

		return new FpFile(
			new EIC(senderIdentification),
			new EIC(receiverIdentification),
			Encoding.UTF8.GetBytes(File.ReadAllText(path)),
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

	public FpPayloadInfo ParsePayload(XDocument document)
	{
		var root = document.Root ?? throw new ValidationException("missing root element");
		XNamespace? ns = root.GetDefaultNamespace();

		var messageType = ParseMessageType(document);

		return new FpPayloadInfo(
			new EIC(ParseSenderIdentification(root, ns)),
			new EIC(ParseReceiverIdentification(root, ns)),
			ParseMessageDateTime(root, ns),
			messageType,
			GetFahrplanhaendlerTyp(messageType));
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

	private FpMessageType ParseMessageType(XDocument document)
	{
		var rootName = document.Root?.Name.LocalName;

		return rootName switch
		{
			"AnomalyReport" => FpMessageType.AnomalyReport,
			"StatusRequest" => FpMessageType.StatusRequest,
			"ConfirmationReport" => FpMessageType.ConfirmationReport,
			"ScheduleMessage" => FpMessageType.Schedule,
			"AcknowledgementMessage" => FpMessageType.Acknowledge,
			_ => throw new ValidationException($"unsupported XmlRoot '{rootName}'.")
		};
	}

	private static string ParseBDEWFulfillmentDate(XElement root, XNamespace? ns, FpMessageType messageType, string dateFromFileName)
	{
		// For acknowledge und status messages we take the date from the filename
		if (messageType == FpMessageType.Acknowledge || messageType == FpMessageType.StatusRequest)
		{
			if (!DateTime.TryParseExact(dateFromFileName, DateTimeFormat.FileDate, System.Globalization.CultureInfo.InvariantCulture,
				    DateTimeStyles.AssumeUniversal, out DateTime toDate))
			{
				throw new ValidationException($"invalid date format '{dateFromFileName}' [format: '{DateTimeFormat.FileDate}']");
			}

			return toDate.ToHyphenDate();
		}
		else
		{
			var scheduleTimeInterval = ParseElementValueOrThrow(root, ns, XmlElementNameScheduleTimeInterval);

			var parts = scheduleTimeInterval.Split('/');
			if (parts.Length != 2) throw new ValidationException($"invalid interval format '{scheduleTimeInterval}' [format: datetime/datetime]");
			var end = parts[1];
			if (!DateTime.TryParse(end, out DateTime toDate))
			{
				throw new ValidationException($"invalid date format '{end}'.");
			}

			return toDate.ToHyphenDate();
		}
	}

	private static string ParseDocumentType(XElement element, XNamespace? ns, FpMessageType messageType)
	{
		return messageType switch
		{
			// Tabelle 6-1 AG-FPM_Regelungen-zum-sicheren-Austausch-im-Fahrplanprozess_v2.1_DE_Final_2023-10-01
			FpMessageType.Acknowledge => BDEWDocumentTypes.A17,
			FpMessageType.AnomalyReport => BDEWDocumentTypes.A16,
			FpMessageType.Schedule or FpMessageType.ConfirmationReport or FpMessageType.StatusRequest => ParseElementValueOrThrow(element, ns,
				XmlElementNameMessageType),
			_ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
		};
	}

	private string ParseDocumentNo(XElement root,
		XNamespace? ns,
		FpMessageType type,
		FpFileName fpFileName)
	{
		return type switch
		{
			FpMessageType.Acknowledge => ParseElementValueOrThrow(root, ns, XmlElementNameReceivingMessageVersion),
			FpMessageType.Schedule => ParseElementValueOrThrow(root, ns, XmlElementNameMessageVersion),
			FpMessageType.ConfirmationReport => ParseElementValueOrThrow(root, ns, XmlElementNameConfirmedMessageVersion),
			FpMessageType.AnomalyReport => fpFileName.Version ?? throw new ValidationException($"missing version"),
			FpMessageType.StatusRequest => "1",
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};
	}

	private static string ParseSenderIdentification(XElement element, XNamespace? ns)
	{
		return ParseElementValueOrThrow(element, ns, XmlElementNameSenderIdentification);
	}

	private static string ParseReceiverIdentification(XElement element, XNamespace? ns)
	{
		return ParseElementValueOrThrow(element, ns, XmlElementNameReceiverIdentification);
	}

	private static string ParseElementValueOrThrow(XElement root, XNamespace? ns, string elementName)
	{
		return root.Element(ns + elementName)?.Attribute("v")?.Value ?? throw new ValidationException($"missing xml element '{elementName}'.");
	}

	private static string ParseSenderRole(XElement element, XNamespace? ns)
	{
		return ParseElementValueOrThrow(element, ns, XmlElementNameSenderRole);
	}

	private static DateTime ParseMessageDateTime(XElement element, XNamespace? ns)
	{
		var value = ParseElementValueOrThrow(element, ns, XmlElementNameMessageDateTime);

		return DateTime.Parse(value).ToUniversalTime();
	}
}