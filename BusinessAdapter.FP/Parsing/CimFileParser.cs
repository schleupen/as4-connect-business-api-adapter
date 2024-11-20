﻿namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

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
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var documentNo = ParseCIMDocumentNoForMessageType(fpFileName.MessageType, document, ns, fpFileName);
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

		string? scheduleTimeInterval = "";
		// For acknowledge und status messages we take the date from the filename
		if (fpFileName.MessageType == FpMessageType.Acknowledge || fpFileName.MessageType == FpMessageType.StatusRequest)
		{
			scheduleTimeInterval = fpFileName.Date;
		}
		else
		{
			var timeInterval = document.Descendants(ns + "schedule_Time_Period.timeInterval").FirstOrDefault();

			if (timeInterval != null)
			{
				var startTimeInterval = timeInterval.Descendants(ns + "start").FirstOrDefault()?.Value;
				var endTimeInterval = timeInterval.Descendants(ns + "end").FirstOrDefault()?.Value;
				if (startTimeInterval is not null && endTimeInterval is not null)
				{
					scheduleTimeInterval = startTimeInterval + "/" + endTimeInterval;
				}
			}

		}

		if (scheduleTimeInterval == null)
		{
			throw new ArgumentException($"Could not retrieve fulfillment date from file {path}.");
		}

		FpBDEWProperties bdewProperties = new FpBDEWProperties(
			documentType,
			documentNo,
			scheduleTimeInterval, // TODO : use YYYY-MM-DD for FulfillmentData
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

	private string? ParseCIMDocumentNoForMessageType(
		FpMessageType type,
		XDocument doc,
		XNamespace? ns,
		FpFileName fpFileName)
	{
		switch (type)
		{
			case FpMessageType.Acknowledge:
				// This should be received_market-document.revisionNumber
				return doc.Descendants(ns + "received_MarketDocument.revisionNumber").First().Value;
			case FpMessageType.Schedule:
				return doc.Descendants(ns + "revisionNumber").First().Value;
			case FpMessageType.ConfirmationReport:
				// This should be confirmed_market-document.revisionNumber
				return doc.Descendants(ns + "revisionNumber").First().Value;
			case FpMessageType.AnomalyReport:
				return fpFileName.Version;
			case FpMessageType.StatusRequest:
				return "1"; // Is this correct?
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}