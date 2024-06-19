// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Receiving
{
	using System;
	using Schleupen.AS4.BusinessAdapter.API;

	/// <summary>
	/// Incoming AS4 message with metadata and EDIFACT payload
	/// </summary>
	public sealed class MpMessage
	{
		public MpMessage(DateTimeOffset createdAt, string bdewDocumentDate, string messageId, PartyInfo partyInfo)
		{
			CreatedAt = createdAt;
			BdewDocumentDate = bdewDocumentDate;
			MessageId = messageId;
			PartyInfo = partyInfo;
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
		public PartyInfo PartyInfo { get; }
	}
}
