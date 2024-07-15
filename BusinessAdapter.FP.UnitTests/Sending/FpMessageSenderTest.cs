namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending;

using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Gateways;
using Schleupen.AS4.BusinessAdapter.FP.Sending;

public partial class FpMessageSenderTest
{
	[Test]
	public async Task SendAvailableMessagesAsync_EmptyDirectory_ShouldDoNothing()
	{
		fixture.SetupEmptyDirectory();

		FpMessageSender sender = fixture.CreateFpMessageSender();

		var sendStatus = await sender.SendAvailableMessagesAsync(CancellationToken.None);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessageCount, Is.EqualTo(0));
		Assert.That(sendStatus.TotalCountOfMessagesInSendDirectory, Is.EqualTo(0));
		Assert.That(sendStatus.SuccessfulMessageCount, Is.EqualTo(0));

		fixture.Mocks.FpFileRepository.Verify(r => r.GetFilesFrom("./Send"), Times.Exactly(1));
		fixture.Mocks.FpFileRepository.VerifyNoOtherCalls();
		fixture.Mocks.BusinessApiGatewayFactory.VerifyNoOtherCalls();
		fixture.VerifyLoggerContainsMessages(LogLevel.Information, "[0/0]", Times.Exactly(1));
	}

	[Test]
	[TestCase(1)]
	[TestCase(5)]
	[TestCase(10)]
	public async Task SendAvailableMessagesAsync_SendingFailed_ShouldUseRetry(int retryCount)
	{
		CancellationToken cancellationToken = new CancellationToken();
		Mock<IBusinessApiGateway> gatewayMock = fixture.SetupSendingFailed(retryCount);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendAvailableMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(retryCount));
		Assert.That(sendStatus.FailedMessageCount, Is.EqualTo(1));
		Assert.That(sendStatus.TotalCountOfMessagesInSendDirectory, Is.EqualTo(1));
		Assert.That(sendStatus.SuccessfulMessageCount, Is.EqualTo(0));

		fixture.Mocks.FpFileRepository.Verify(r => r.GetFilesFrom("./Send"), Times.Exactly(1));
		fixture.Mocks.FpFileRepository.VerifyNoOtherCalls();
		fixture.VerifyLoggerContainsMessages(LogLevel.Information, "[0/1]", Times.Exactly(1));
		for (int i = 1; i <= retryCount; i++)
		{
			fixture.VerifyLoggerContainsMessages(LogLevel.Warning,
				$"Error while sending messages - executing retry [{i}/{retryCount}] with '1' messages", Times.Exactly(1));
		}

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(retryCount + 1));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_TooManyConnections_ShouldAbortSending()
	{
		var cancellationToken = new CancellationToken();
		var gatewayMock = fixture.SetupTooManyConnection(cancellationToken);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendAvailableMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessageCount, Is.EqualTo(1));
		Assert.That(sendStatus.TotalCountOfMessagesInSendDirectory, Is.EqualTo(1));
		Assert.That(sendStatus.SuccessfulMessageCount, Is.EqualTo(0));

		fixture.Mocks.FpFileRepository.Verify(r => r.GetFilesFrom("./Send"), Times.Exactly(1));
		fixture.Mocks.FpFileRepository.VerifyNoOtherCalls();
		fixture.VerifyLoggerContainsMessages(LogLevel.Information, "[0/1]", Times.Exactly(1));
		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(1));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_SendingIsSuccessful_ShouldReturnCorrectSendStatus()
	{
		var cancellationToken = new CancellationToken();

		var successfulParsedFiles = 23;
		var failedParsedFiles = 10;
		var totalMessageCount = successfulParsedFiles + failedParsedFiles;

		var gatewayMock = fixture.SetupSendingSuccessful(cancellationToken, successfulParsedFiles, failedParsedFiles);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendAvailableMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessageCount, Is.EqualTo(failedParsedFiles));
		Assert.That(sendStatus.TotalCountOfMessagesInSendDirectory, Is.EqualTo(totalMessageCount));
		Assert.That(sendStatus.SuccessfulMessageCount, Is.EqualTo(successfulParsedFiles));

		fixture.VerifyLoggerContainsMessages(LogLevel.Information, $"[{successfulParsedFiles}/{totalMessageCount}]", Times.Exactly(1));
		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(successfulParsedFiles));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_SendingIsSuccessful_ShouldTakeInCountMessageLimit()
	{
		var cancellationToken = new CancellationToken();

		var successfulParsedFiles = 100;
		var failedParsedFiles = 5;
		var totalMessageCount = successfulParsedFiles + failedParsedFiles;
		var messageLimit = 10;

		var gatewayMock = fixture.SetupSendingSuccessfulWithLimit(cancellationToken, successfulParsedFiles, failedParsedFiles, messageLimit);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendAvailableMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessageCount, Is.EqualTo(failedParsedFiles));
		Assert.That(sendStatus.TotalCountOfMessagesInSendDirectory, Is.EqualTo(totalMessageCount));
		Assert.That(sendStatus.SuccessfulMessageCount, Is.EqualTo(messageLimit));

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(messageLimit));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_MultipleMessagesFromOneSender_ShouldInstantiateSenderSpecificGateway()
	{
		var cancellationToken = new CancellationToken();
		var gatewayMock = fixture.SetupMultipleMessagesFromOnlyOneSender(cancellationToken, 23, 10);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		await sender.SendAvailableMessagesAsync(cancellationToken);

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(23));
		fixture.Mocks.BusinessApiGatewayFactory.Verify(f => f.CreateGateway(It.IsAny<Party>()), Times.Exactly(1));
	}
}