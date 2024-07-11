namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.API;

// TODO Test
public record SendStatus(int MessagesInSendDirectoryCount, int MessageLimitCount)
{
	private readonly List<FpOutboxMessage> successfulMessages = new();
	private readonly Dictionary<Guid, Tuple<FpOutboxMessage, Exception>> failedMessages = new();
	private bool abortedDueToTooManyConnections;
	private int iteration = 0;

	public void NewIteration()
	{
		this.iteration++;
	}

	public int Iteration => this.iteration;

	public void AddBusinessApiResponse(BusinessApiResponse<FpOutboxMessage> response, ILogger logger)
	{
		if (response.WasSuccessful)
		{
			logger.LogDebug("message for '{FileName}' send successful", response.Message.FileName);
			failedMessages.Remove(response.Message.MessageId);
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
		failedMessages[message.MessageId] = new Tuple<FpOutboxMessage, Exception>(message, exception);
	}

	public void AbortedDueToTooManyConnections()
	{
		this.abortedDueToTooManyConnections = true;
	}

	public int FailedMessageCount => this.failedMessages.Count;

	public int SuccessfulMessageCount => this.successfulMessages.Count;

	public void ThrowIfRetryIsNeeded()
	{
		if (this.FailedMessageCount != 0)
		{
			throw new AggregateException("There was at least one error. Details can be found in the inner exceptions.", this.failedMessages.Select(x => x.Value.Item2).ToArray());
		}
	}

	public void LogTo(ILogger logger)
	{
		if (abortedDueToTooManyConnections)
		{
			logger.LogWarning("A 429 TooManyRequests status code was encountered while sending the messages which caused the sending to end before all messages could be sent.");
		}

		foreach (var failedMessage in this.failedMessages)
		{
			logger.LogWarning(failedMessage.Value.Item2, "Failed to Send message for '{FilePath}'", failedMessage.Value.Item1.FilePath);
		}

		logger.LogInformation("Messages [{SuccessfulMessagesCount}/{MessageInSendDirectoryCount}] successful send. [Limit: {MessageLimitCount} Failed: {FailedMessagesCount}]",
			SuccessfulMessageCount,
			MessagesInSendDirectoryCount,
			MessageLimitCount,
			FailedMessageCount);
	}

	public List<FpOutboxMessage> GetUnsentMessages()
	{
		return this.failedMessages.Select(x => x.Value.Item1).ToList();
	}
}