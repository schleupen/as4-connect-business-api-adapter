// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Sending
{
	using System;
	using Schleupen.AS4.BusinessAdapter.API;

	/// <summary>
	/// Outgoing AS4 message with metadata and edifact message.
	/// </summary>
	public class OutboxMessage
	{
		public OutboxMessage(ReceivingParty receiver, string senderMessageId, string bdewDocumentNumber, string bdewDocumentType, byte[] payload, string filename, DateTimeOffset documentDate)
		{
			Receiver = receiver;
			SenderMessageId = senderMessageId;
			BdewDocumentNumber = bdewDocumentNumber;
			BdewDocumentType = bdewDocumentType;
			Payload = payload;
			FileName = filename;
			DocumentDate = documentDate;
		}

		/// <summary>
		/// Receiving party of the message.
		/// </summary>
		public ReceivingParty Receiver { get; }

		/// <summary>
		/// EDIFACT payload.
		/// </summary>
#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
		public byte[] Payload { get; }
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben

		/// <summary>
		/// Name of the document type (e.g. MSCONS).
		/// </summary>
		public string BdewDocumentType { get; }

		/// <summary>
		/// Document number.
		/// </summary>
		public string BdewDocumentNumber { get; }

		/// <summary>
		/// Optional Id which the sender of the message may individually add. This Id should originate from the connected bussiness application and allows to identify the message in AS4 Connect later on.
		/// </summary>
		public string? SenderMessageId { get; }

		/// <summary>
		/// The file name of the EDIFACT file.
		/// </summary>
		public string FileName { get; }

		/// <summary>
		/// Time of creation of the EDIFACT file.
		/// </summary>
		public DateTimeOffset DocumentDate { get; }
	}
}
