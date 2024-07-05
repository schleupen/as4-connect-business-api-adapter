// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using BusinessApi;

	/// <summary>
	/// Wrapper for the business API client.
	/// </summary>
	public interface IClientWrapper
	{
		/// <summary>
		/// Adds a message to the outbox.
		/// </summary>
		/// <param name="receiverId">The identification number of the receiving party.</param>
		/// <param name="receiverIdType">The type of the identification number of the receiving party. (e.g. BDEW)</param>
		/// <param name="payload">The payload of the message.</param>
		/// <param name="bdewDocumentType">The document type of the message. (e.g. MSCONS)</param>
		/// <param name="bdewDocumentNo">The document number of the message.</param>
		/// <param name="bdewDocumentDate">The document date of the message</param>
		/// <param name="messageId">The identification of the message.</param>
		/// <param name="senderMessageId">The sender identification number of the message.</param>
		/// <returns></returns>
		Task V1MpMessagesOutboxPostAsync(string receiverId, PartyIdTypeDto receiverIdType, FileParameter payload, string bdewDocumentType, string bdewDocumentNo, string bdewDocumentDate, Guid messageId, string? senderMessageId);

		/// <summary>
		/// Queries messages in the inbox up to the given limit.
		/// </summary>
		/// <param name="limit">The message limit.</param>
		/// <returns>The messages in the inbox.</returns>
		Task<QueryInboxMessagesResponseDto> V1MpMessagesInboxAsync(int limit);

		/// <summary>
		/// Queries the payload of the message with the given identifier.
		/// </summary>
		/// <param name="messageId">Identifier of the message.</param>
		/// <returns>The payload of the message.</returns>
		Task<FileResponse> V1MpMessagesInboxPayloadAsync(Guid messageId);

		/// <summary>
		/// Acknowledges the message with the given identifier.
		/// </summary>
		/// <param name="messageId">The identifier of the message.</param>
		/// <param name="messageAcknowledgedRequestDto">The request for the acknowledgment.</param>
		/// <returns></returns>
		Task V1MpMessagesInboxAcknowledgementAsync(Guid messageId, MessageAcknowledgedRequestDto messageAcknowledgedRequestDto);
	}
}