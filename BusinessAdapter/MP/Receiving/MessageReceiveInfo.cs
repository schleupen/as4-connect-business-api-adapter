// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Receiving
{
	using System.Collections.Generic;

	/// <summary>
	/// Contains the result of a message query.
	/// </summary>
	public sealed class MessageReceiveInfo
	{
		private readonly As4Message[] availableMessages;
		private readonly HashSet<InboxMessage> confirmableMessages = [];

		public MessageReceiveInfo(As4Message[] availableMessages)
		{
			this.availableMessages = availableMessages;
		}

		/// <summary>
		/// Contains the successfully processed messages.
		/// </summary>
		public IReadOnlyCollection<InboxMessage> ConfirmableMessages => confirmableMessages;

		/// <summary>
		/// Saves the information if too many calls occured. This is not set by the API.
		/// </summary>
		public bool HasTooManyRequestsError { get; set; }

		/// <summary>
		/// Returns the available messages.
		/// </summary>
		public As4Message[] GetAvailableMessages()
		{
			return availableMessages;
		}

		/// <summary>
		/// Adds a message to the list of successfully processed messages.
		/// </summary>
		/// <param name="message"></param>
		public void AddReceivedEdifactMessage(InboxMessage message)
		{
			confirmableMessages.Add(message);
		}
	}
}
