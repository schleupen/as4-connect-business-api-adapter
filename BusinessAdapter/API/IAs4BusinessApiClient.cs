// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System;
	using System.Threading.Tasks;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	/// <summary>
	/// Client of the access to AS4 Connect.
	/// </summary>
	public interface IAs4BusinessApiClient : IDisposable
	{
		/// <summary>
		/// Adds an outgoing message including the EDIFACT content to be send to the foreign market partner.
		/// </summary>
		/// <returns>Contains information regarding the success of the sending process.</returns>
		Task<MessageResponse<OutboxMessage>> SendMessageAsync(OutboxMessage message);

		/// <summary>
		/// Queries the available messages ready to be received.
		/// </summary>
		/// <param name="limit">default: 50, min: 1, max: 1000</param>
		/// <returns>The Metadata of the available messages.</returns>
		Task<MessageReceiveInfo> QueryAvailableMessagesAsync(int limit = 50);

		/// <summary>
		/// Queries EDIFACT files for the given AS4 message..
		/// </summary>
		/// <param name="as4Message">The Metadata of the message.</param>
		/// <returns>The EDIFACT file.</returns>
		Task<MessageResponse<InboxMessage>> ReceiveMessageAsync(As4Message as4Message);

		/// <summary>
		/// Confirms that a message was successfully received.
		/// </summary>
		/// <param name="message">The received message that should be acknowledged.</param>
		/// <returns>Whether the acknowledgement was successful.</returns>
		Task<MessageResponse<bool>> AcknowledgeReceivedMessageAsync(InboxMessage message);
	}
}