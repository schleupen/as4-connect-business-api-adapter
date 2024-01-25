// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using System;
	using Schleupen.AS4.BusinessAdapter.API;

	/// <summary>
	/// Incoming AS4 message with metadata and EDIFACT payload
	/// </summary>
	public sealed class As4Message
	{
		public As4Message(DateTimeOffset createdAt, string bdewDocumentDate, string messageId, Partyinfo partyinfo)
		{
			CreatedAt = createdAt;
			BdewDocumentDate = bdewDocumentDate;
			MessageId = messageId;
			PartyInfo = partyinfo;
		}

		/// <summary>
		/// The identification of the message.
		/// </summary>
		public string MessageId { get; }

		/// <summary>
		/// The timestamp of creation in AS4 Connect.
		/// </summary>
		public DateTimeOffset CreatedAt { get; }

		/// <summary>
		/// The document date of the EDIFACT message.
		/// </summary>
		public string BdewDocumentDate { get; }

		/// <summary>
		/// Contains information about the sending and receiving party of the message.
		/// </summary>
		public Partyinfo PartyInfo { get; }
	}
}
