namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending;

using System.Reflection;
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

		var sendStatus = await sender.SendMessagesAsync(CancellationToken.None);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessages.Count, Is.EqualTo(0));
		Assert.That(sendStatus.TotalMessageCount, Is.EqualTo(0));
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(0));

		fixture.Mocks.FpFileRepository.Verify(r => r.GetFilesFrom(TestData.SendDir), Times.Exactly(1));
		fixture.Mocks.FpFileRepository.VerifyNoOtherCalls();
		fixture.Mocks.BusinessApiGatewayFactory.VerifyNoOtherCalls();
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
		var sendStatus = await sender.SendMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(retryCount));
		Assert.That(sendStatus.FailedMessages.Count, Is.EqualTo(1));
		Assert.That(sendStatus.TotalMessageCount, Is.EqualTo(1));
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(0));

		fixture.Mocks.FpFileRepository.Verify(r => r.GetFilesFrom(TestData.SendDir), Times.Exactly(1));
		fixture.Mocks.FpFileRepository.VerifyNoOtherCalls();
		for (int i = 1; i <= retryCount; i++)
		{
			fixture.VerifyLoggerContainsMessages(LogLevel.Warning, $"Error while sending messages - retry {i}/{retryCount} with '1' messages",
				Times.Exactly(1));
		}

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(retryCount + 1));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_TooManyConnections_ShouldAbortSending()
	{
		var cancellationToken = new CancellationToken();
		var gatewayMock = fixture.SetupTooManyConnection(cancellationToken);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessages.Count, Is.EqualTo(1));
		Assert.That(sendStatus.TotalMessageCount, Is.EqualTo(1));
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(0));

		fixture.Mocks.FpFileRepository.Verify(r => r.GetFilesFrom(TestData.SendDir), Times.Exactly(1));
		fixture.Mocks.FpFileRepository.VerifyNoOtherCalls();
		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(1));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_SendingIsSuccessful_ShouldReturnCorrectSendStatus()
	{
		var cancellationToken = new CancellationToken();

		var successfulParsedFiles = 23;
		var failedParsedFiles = 10;
		var totalMessageCount = successfulParsedFiles + failedParsedFiles;

		var gatewayMock = fixture.SetupSendingSuccessful(successfulParsedFiles, failedParsedFiles, cancellationToken);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessages.Count, Is.EqualTo(failedParsedFiles));
		Assert.That(sendStatus.TotalMessageCount, Is.EqualTo(totalMessageCount));
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(successfulParsedFiles));

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(successfulParsedFiles));
		fixture.Mocks.FpFileRepository.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Exactly(successfulParsedFiles));
		foreach (var successFilePath in sendStatus.SuccessfulMessages.Select(x => x.FilePath))
		{
			fixture.Mocks.FpFileRepository.Verify(x => x.DeleteFile(successFilePath), Times.Exactly(1));
		}
	}

	[Test]
	public async Task SendAvailableMessagesAsync_MessageLimitIsLowerThanMessageInDirectory_ShouldOnlySendLimitedMessageCount()
	{
		var cancellationToken = new CancellationToken();

		var successfulParsedFiles = 100;
		var failedParsedFiles = 5;
		var totalMessageCount = successfulParsedFiles + failedParsedFiles;
		var messageLimit = 10;

		var gatewayMock = fixture.SetupSendingSuccessfulWithLimit(successfulParsedFiles, failedParsedFiles, messageLimit, cancellationToken);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		var sendStatus = await sender.SendMessagesAsync(cancellationToken);

		Assert.That(sendStatus.RetryIteration, Is.EqualTo(0));
		Assert.That(sendStatus.FailedMessages.Count, Is.EqualTo(failedParsedFiles));
		Assert.That(sendStatus.TotalMessageCount, Is.EqualTo(totalMessageCount));
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(messageLimit));

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(messageLimit));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_MultipleMessagesFromOneSender_ShouldInstantiateSenderSpecificGateway()
	{
		var cancellationToken = new CancellationToken();
		var gatewayMock = fixture.SetupMultipleMessagesFromOnlyOneSender(23, 10, cancellationToken);

		FpMessageSender sender = fixture.CreateFpMessageSender();
		await sender.SendMessagesAsync(cancellationToken);

		gatewayMock.Verify(x => x.SendMessageAsync(It.IsAny<FpOutboxMessage>(), cancellationToken), Times.Exactly(23));
		fixture.Mocks.BusinessApiGatewayFactory.Verify(f => f.CreateGateway(It.IsAny<Party>()), Times.Exactly(1));
	}

	[Test]
	public async Task SendAvailableMessagesAsync_SenderGatewayCreationFails_AllMessagesFromSenderShouldBeFailedWithException()
	{
		var exception = new InvalidOperationException("missing certs");
		fixture.SetupSenderGatewayCreationFails(exception);

		FpMessageSender sender = fixture.CreateFpMessageSender();

		var result = await sender.SendMessagesAsync(CancellationToken.None);

		Assert.That(result.FailedMessages.Select(x => x.Exception), Is.All.EqualTo(exception));
	}
}