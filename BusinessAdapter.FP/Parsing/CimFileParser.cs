﻿namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Text;
using System.Xml.Linq;

public class CimFileParser : IFpFileSpecificParser
{
	public IFpFile Parse(XDocument document, string filename, string path)
	{
		string xmlData = File.ReadAllText(path);
		byte[] content = Encoding.UTF8.GetBytes(xmlData);
		FpFileName fpFileName = FpFileName.Parse(filename);
		XNamespace? ns = document.Root?.GetDefaultNamespace();

		var documentNo = ParseCIMDocumentNoForMessageType(fpFileName.MessageType, document, ns, fpFileName);
		if (documentNo == null)
		{
			throw new ArgumentException($"Could not document number from file {path}.");
		}

		var documentIdentification = document.Descendants(ns + "mRID").First().Attribute("v").Value;

		var senderIdentification = document.Descendants(ns + "sender_MarketParticipant.mRID").First().Attribute("v").Value;
		if (senderIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
		}

		var senderRole = document.Descendants(ns + "sender_MarketParticipant.marketRole.type").First().Attribute("v").Value;
		if (senderRole == null)
		{
			throw new ArgumentException($"Could not retrieve sender role from file {path}.");
		}

		var receiverIdentification = document.Descendants(ns + "sender_MarketParticipant.mRID").First().Attribute("v").Value;
		if (receiverIdentification == null)
		{
			throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
		}

		var receiverRole = document.Descendants(ns + "sender_MarketParticipant.marketRole.type").First().Attribute("v").Value;
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
			var startTimeInterval = document.Descendants(ns + "sender_MarketParticipant.start").First().Attribute("v")?.Value;
			var endTimeInterval = document.Descendants(ns + "sender_MarketParticipant.end").First().Attribute("v")?.Value;

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
			fpFileName.MessageType.ToString(),
			scheduleTimeInterval,
			senderIdentification,
			senderRole,
			path,
			senderIdentification,
			receiverIdentification,
			receiverRole);
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
				return doc.Descendants(ns + "revisionNumber").First().Attribute("v").Value;
			case FpMessageType.Schedule:
				return doc.Descendants(ns + "revisionNumber").First().Attribute("v").Value;
			case FpMessageType.Confirmation:
				// This should be confirmed_market-document.revisionNumber
				return doc.Descendants(ns + "revisionNumber").First().Attribute("v").Value;
			case FpMessageType.Anomaly:
				return fpFileName.Version;
			case FpMessageType.Status:
				return "1"; // Is this correct?
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}