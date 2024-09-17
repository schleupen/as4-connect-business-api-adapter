namespace Schleupen.AS4.BusinessAdapter.FP.Sending;

using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.API;

public class SendStatus(DirectoryResult directoryResult) : ISendStatus
{
	private readonly List<FpOutboxMessage> successfulSendMessages = new();
	private readonly Dictionary<Guid, FailedFpOutboxMessage> failedSendMessages = new();
	private int retryIteration = 0;

	public bool AbortedDueToTooManyConnections { get; private set; }

	public void NewRetry()
	{
		this.retryIteration++;
	}

	public int RetryIteration => this.retryIteration;

	public int TotalMessageCount => directoryResult.TotalFileCount;

	public void AddBusinessApiResponse(BusinessApiResponse<FpOutboxMessage> response, ILogger logger)
	{
		if (response.WasSuccessful)
		{
			logger.LogDebug("message for '{FileName}' send successful", response.Message.FileName);
			failedSendMessages.Remove(response.Message.MessageId); // originally failed messages which has been fixed by retry
			AddSuccessfulSendMessage(response.Message);
		}
		else
		{
			AddFailure(response.Message, response.ApiException!, logger);
		}
	}

	private void AddSuccessfulSendMessage(FpOutboxMessage message)
	{
		successfulSendMessages.Add(message);
	}

	public void AddFailure(FpOutboxMessage message, Exception exception, ILogger logger)
	{
		logger.LogDebug(exception, "Failed to send message for '{FileName}'", message.FileName);
		failedSendMessages[message.MessageId] = new FailedFpOutboxMessage(message, exception);
	}

	public void AbortDueToTooManyConnections()
	{
		this.AbortedDueToTooManyConnections = true;
	}

	public void ThrowIfRetryIsNeeded()
	{
		if (this.AbortedDueToTooManyConnections)
		{
			return; // send retry will not fix this problem, so throwing the Exception for Retry doesn't make sense
		}

		if (this.failedSendMessages.Count != 0)
		{
			throw new AggregateException("There was at least one error. Details can be found in the inner exceptions.",
				this.failedSendMessages.Select(x => x.Value.Exception).ToArray());
		}
	}

	public void LogTo(ILogger logger)
	{
		if (AbortedDueToTooManyConnections)
		{
			logger.LogWarning("A 429 TooManyRequests status code was encountered while sending the messages which caused the sending to end before all messages could be sent.");
		}

		foreach (var failedMessage in this.failedSendMessages)
		{
			logger.LogWarning(failedMessage.Value.Exception, "Failed to send message for '{FilePath}'", failedMessage.Value.Message.FilePath);
		}

		foreach (var failedParsedFile in directoryResult.FailedFiles)
		{
			logger.LogWarning(failedParsedFile.Exception, "Failed to parse file '{FilePath}'", failedParsedFile.Path);
		}

		logger.LogInformation(
			"Messages {SuccessfulMessagesCount}/{MessageInSendDirectoryCount} send successful.",
			SuccessfulMessages.Count,
			TotalMessageCount);
	}

	public List<FpOutboxMessage> GetUnsentMessagesForRetry()
	{
		return this.failedSendMessages.Select(x => x.Value.Message).ToList();
	}

	public IReadOnlyCollection<FpOutboxMessage> SuccessfulMessages => successfulSendMessages.AsReadOnly();

	public IReadOnlyCollection<FailedFpOutboxMessage> FailedMessages => BuildAllFailedFpMessages();

	private IReadOnlyCollection<FailedFpOutboxMessage> BuildAllFailedFpMessages()
	{
		return this.failedSendMessages.Values
			.Concat(directoryResult.FailedFiles.Select(x => new FailedFpOutboxMessage(x.Path, x.Exception)))
			.ToList()
			.AsReadOnly();
	}
}