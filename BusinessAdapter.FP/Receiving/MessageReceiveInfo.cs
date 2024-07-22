namespace Schleupen.AS4.BusinessAdapter.FP.Receiving;

/// <summary>
/// Contains the result of a message query.
/// </summary>
public sealed class MessageReceiveInfo
{
    private readonly As4FpMessage[] availableMessages;
    private readonly HashSet<InboxFpMessage> confirmableMessages = [];

    public MessageReceiveInfo(As4FpMessage[] availableMessages)
    {
        this.availableMessages = availableMessages;
    }

    /// <summary>
    /// Contains the successfully processed messages.
    /// </summary>
    public IReadOnlyCollection<InboxFpMessage> ConfirmableMessages => confirmableMessages;

    /// <summary>
    /// Saves the information if too many calls occured. This is not set by the API.
    /// </summary>
    public bool HasTooManyRequestsError { get; set; }

    /// <summary>
    /// Returns the available messages.
    /// </summary>
    public As4FpMessage[] GetAvailableMessages()
    {
        return availableMessages;
    }

    /// <summary>
    /// Adds a message to the list of successfully processed messages.
    /// </summary>
    /// <param name="mpMessage"></param>
    public void AddReceivedXmlMessage(InboxFpMessage mpMessage)
    {
        confirmableMessages.Add(mpMessage);
    }
}