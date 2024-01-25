// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using System;
	using System.Security.Cryptography;
	using Schleupen.AS4.BusinessAdapter.API;

	/// <summary>
	/// Incoming AS4 message with metadata and EDIFACT payload.
	/// </summary>
	public class InboxMessage
	{
		public InboxMessage(string messageId, DateTimeOffset createdAt, string bdewDocumentDate, SendingParty sender, ReceivingParty receiver, string? edifactContent, byte[]? zippedContent)
		{
			MessageId = messageId;
			CreatedAt = createdAt;
			BdewDocumentDate = bdewDocumentDate;
			Sender = sender;
			Receiver = receiver;
			EdifactContent = edifactContent;
			ZippedContent = zippedContent;
		}

		/// <summary>
		/// The Identifier of the message.
		/// </summary>
		public string? MessageId { get; }

		/// <summary>
		/// The timestamp when the message was generated in AS4 Connect.
		/// </summary>
		public DateTimeOffset CreatedAt { get; }

		/// <summary>
		/// The document date of the EDIFACT message.
		/// </summary>
		public string BdewDocumentDate { get; }

		/// <summary>
		/// The sending party of the AS4 message.
		/// </summary>
		public SendingParty Sender { get; }

		/// <summary>
		/// The receiving party of the AS4 message.
		/// </summary>
		public ReceivingParty Receiver { get; }

		/// <summary>
		/// Decompressed content of the EDIFACT file.
		/// </summary>
		public string? EdifactContent { get; }

		/// <summary>
		/// Zipped content of the EDIFACT message.
		/// </summary>
		private byte[]? ZippedContent { get; }

		internal string? ContentHashSha256
		{
			get
			{
				if (ZippedContent == null || ZippedContent.Length == 0)
				{
					return null;
				}

				// The hash has to be calculated on the zipped payload
				byte[] ediHash = SHA256.HashData(ZippedContent);
				return Convert.ToBase64String(ediHash);
			}
		}

		public override bool Equals(object? obj)
		{
			return obj is InboxMessage other && !string.IsNullOrEmpty(other.MessageId) && other.MessageId.Equals(MessageId, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return MessageId != null ? 376 + MessageId.GetHashCode(StringComparison.OrdinalIgnoreCase) : 0;
		}
	}
}
