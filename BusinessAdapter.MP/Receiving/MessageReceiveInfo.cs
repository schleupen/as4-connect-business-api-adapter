// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Receiving
{
	using System.Collections.Generic;

	/// <summary>
	/// Contains the result of a message query.
	/// </summary>
	public sealed class MessageReceiveInfo
	{
		private readonly MpMessage[] availableMessages;
		private readonly HashSet<InboxMpMessage> confirmableMessages = [];

		public MessageReceiveInfo(MpMessage[] availableMessages)
		{
			this.availableMessages = availableMessages;
		}

		/// <summary>
		/// Contains the successfully processed messages.
		/// </summary>
		public IReadOnlyCollection<InboxMpMessage> ConfirmableMessages => confirmableMessages;

		/// <summary>
		/// Saves the information if too many calls occured. This is not set by the API.
		/// </summary>
		public bool HasTooManyRequestsError { get; set; }

		/// <summary>
		/// Returns the available messages.
		/// </summary>
		public MpMessage[] GetAvailableMessages()
		{
			return availableMessages;
		}

		/// <summary>
		/// Adds a message to the list of successfully processed messages.
		/// </summary>
		/// <param name="mpMessage"></param>
		public void AddReceivedEdifactMessage(InboxMpMessage mpMessage)
		{
			confirmableMessages.Add(mpMessage);
		}
	}
}
