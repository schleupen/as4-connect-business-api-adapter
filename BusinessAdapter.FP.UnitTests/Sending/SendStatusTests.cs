﻿namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending;

using System.Collections.Immutable;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.FP.Sending;

public partial class SendStatusTest
{
	[Test]
	public void Ctor_ShouldInitializeCorrectly()
	{
		SendStatus status = fixture.CreateSendStatusObject();

		Assert.That(status.TotalMessageCount, Is.EqualTo(0));
		Assert.That(status.SuccessfulMessages.Count, Is.EqualTo(0));
		Assert.That(status.FailedMessages.Count, Is.EqualTo(0));
		Assert.That(status.RetryIteration, Is.EqualTo(0));
		Assert.DoesNotThrow(() => status.ThrowIfRetryIsNeeded());
		Assert.That(status.AbortedDueToTooManyConnections, Is.False);
	}

	[Test]
	public void AddFailure_SingleFailure_RetryIsNeeded()
	{
		SendStatus status = fixture.CreateSendStatusObject();

		var exception = new InvalidOperationException("");

		status.AddFailure(fixture.Data.FailedOutboundMessage, exception, fixture.Mocks.LoggerMock.Object);

		Assert.That(status.FailedMessages.Count, Is.EqualTo(1));
		Assert.That(status.GetUnsentMessagesForRetry()[0], Is.EqualTo(fixture.Data.FailedOutboundMessage));
		Assert.Throws<AggregateException>(() => status.ThrowIfRetryIsNeeded());
	}

	[Test]
	public void AddBusinessApiResponse_WasSuccessful_WithOriginallyFailedMessage_RetryIsNotNeededAnymore()
	{
		SendStatus status = fixture.CreateSendStatusObject();

		var exception = new InvalidOperationException("");

		status.AddFailure(fixture.Data.FailedOutboundMessage, exception, fixture.Mocks.LoggerMock.Object);
		status.NewRetry();
		status.AddBusinessApiResponse(new BusinessApiResponse<FpOutboxMessage>(true, fixture.Data.FailedOutboundMessage),
			fixture.Mocks.LoggerMock.Object);

		Assert.That(status.RetryIteration, Is.EqualTo(1));
		Assert.That(status.FailedMessages.Count, Is.EqualTo(0));
		Assert.That(status.SuccessfulMessages.Count, Is.EqualTo(1));
		Assert.That(status.GetUnsentMessagesForRetry(), Is.Empty);
		Assert.DoesNotThrow(() => status.ThrowIfRetryIsNeeded());
	}

	[Test]
	public void GetUnsentMessagesForRetry_ShouldReturnFailedMessage()
	{
		SendStatus status = fixture.CreateSendStatusObject();

		var exception = new InvalidOperationException("");

		status.AddFailure(fixture.Data.FailedOutboundMessage, exception, fixture.Mocks.LoggerMock.Object);

		var unsent = status.GetUnsentMessagesForRetry();

		Assert.That(unsent, Is.Not.Empty);
		Assert.That(unsent[0], Is.EqualTo(fixture.Data.FailedOutboundMessage));
	}

	[Test]
	public void AbortedDueToTooManyConnections_WithFailedMessages_RetryIsUnnecessary()
	{
		SendStatus status = fixture.CreateSendStatusObject();

		status.AddFailure(fixture.Data.FailedOutboundMessage, new InvalidOperationException(""), fixture.Mocks.LoggerMock.Object);
		status.AbortDueToTooManyConnections();

		Assert.DoesNotThrow(() => status.ThrowIfRetryIsNeeded());
		Assert.That(status.AbortedDueToTooManyConnections, Is.True);
	}

	[Test]
	public void AddBusinessApiResponse_FailedResponse_RetryIsNeeded()
	{
		SendStatus status = fixture.CreateSendStatusObject();

		var outboundMessage = fixture.Data.FailedOutboundMessage;

		var response = new BusinessApiResponse<FpOutboxMessage>(false,
			outboundMessage,
			HttpStatusCode.Forbidden,
			new InvalidOperationException(""));

		status.AddBusinessApiResponse(response, fixture.Mocks.LoggerMock.Object);

		Assert.That(status.FailedMessages.Count, Is.EqualTo(1));
		Assert.Throws<AggregateException>(() => status.ThrowIfRetryIsNeeded());
	}

	[Test]
	public void FailedMessagesInDirectoryResult_ShouldNotBeTakenForRetry()
	{
		var failedFileInDirectory = new FailedFile("idk/failedFile.xml", new InvalidOperationException("idk"));

		DirectoryResult directoryResult = new DirectoryResult("idk",
			ImmutableList<FpFile>.Empty,
			new List<FailedFile>() { failedFileInDirectory }.ToImmutableList());

		SendStatus status = new SendStatus(directoryResult);

		Assert.That(status.FailedMessages.Count, Is.EqualTo(1));
		Assert.That(status.GetUnsentMessagesForRetry(), Has.Count.EqualTo(0));
		Assert.DoesNotThrow(() => status.ThrowIfRetryIsNeeded());
	}

	[Test]
	public void FailedMessages_FailedResponse_FailedMessagesInDirectory_FailedMessagesContainsBothFailures()
	{
		var failedParseException = new InvalidOperationException("parsing failed");
		var failedFileInDirectory = new FailedFile("idk/failedFile.xml", failedParseException);

		DirectoryResult directoryResult = new DirectoryResult("idk",
			ImmutableList<FpFile>.Empty,
			new List<FailedFile>() { failedFileInDirectory }.ToImmutableList());

		SendStatus status = new SendStatus(directoryResult);

		var outboundMessage = fixture.Data.FailedOutboundMessage;

		var sendFailedException = new InvalidOperationException("send failed");
		var response = new BusinessApiResponse<FpOutboxMessage>(false,
			outboundMessage,
			HttpStatusCode.Forbidden,
			sendFailedException);

		status.AddBusinessApiResponse(response, fixture.Mocks.LoggerMock.Object);

		Assert.That(status.FailedMessages.Count, Is.EqualTo(2));
		Assert.That(status.FailedMessages.Select(x => x.Exception).ToList(), Does.Contain(failedParseException));
		Assert.That(status.FailedMessages.Select(x => x.Exception).ToList(), Does.Contain(sendFailedException));
		Assert.Throws<AggregateException>(() => status.ThrowIfRetryIsNeeded());
	}
}