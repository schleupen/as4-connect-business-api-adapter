namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.API;

// TODO Test
public record SendStatus(int MessagesInSendDirectoryCount, int MessageLimitCount)
{
	private readonly List<FpOutboxMessage> successfulMessages = new();
	private readonly List<Tuple<FpOutboxMessage, Exception>> failedMessages = new();
	private bool abortedDueToTooManyConnections;

	public void AddBusinessApiResponse(BusinessApiResponse<FpOutboxMessage> response, ILogger logger)
	{
		if (response.WasSuccessful)
		{
			AddSuccessfulSendMessage(response.Message);
		}
		else
		{
			AddFailure(response.Message, response.ApiException!, logger);
		}
	}

	private void AddSuccessfulSendMessage(FpOutboxMessage message)
	{
		successfulMessages.Add(message);
	}

	public void AddFailure(FpOutboxMessage message, Exception exception, ILogger logger)
	{
		logger.LogWarning(exception, "Failed to send message for '{FileName}'", message.FileName);
		failedMessages.Add(new Tuple<FpOutboxMessage, Exception>(message, exception));
	}

	public void AbortedDueToTooManyConnections()
	{
		this.abortedDueToTooManyConnections = true;
	}

	public int FailedMessageCount => this.failedMessages.Count;

	public int SuccessfulMessageCount => this.successfulMessages.Count;

	public void LogTo(ILogger<SendMessageAdapterController> logger)
	{
		if (abortedDueToTooManyConnections)
		{
			logger.LogWarning("A 429 TooManyRequests status code was encountered while sending the messages which caused the sending to end before all messages could be sent.");
		}
		foreach (var failedMessage in this.failedMessages)
		{
			logger.LogWarning(failedMessage.Item2, "Failed to Send message for '{FilePath}'",failedMessage.Item1.FilePath);
		}
		logger.LogInformation("Messages {SuccessfulMessagesCount}/{MessageInSendDirectoryCount} successful send. [Limit: {MessageLimitCount} Failed: {FailedMessagesCount}]",
			SuccessfulMessageCount,
			MessagesInSendDirectoryCount,
			MessageLimitCount,
			FailedMessageCount);
	}
}