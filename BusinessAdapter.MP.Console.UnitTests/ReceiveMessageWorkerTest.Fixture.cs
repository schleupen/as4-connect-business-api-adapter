// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Microsoft.Extensions.Logging;
using Moq;
using Schleupen.AS4.BusinessAdapter.MP;

internal sealed partial class ReceiveMessageWorkerTest
{
	private sealed class Fixture : IDisposable
	{
		private readonly MockRepository mockRepository = new(MockBehavior.Strict);
		private readonly Mock<ILogger<ReceiveMessageWorker>> loggerMock;
		private readonly Mock<IReceiveMessageAdapterController> receiveControllerMock;

		public Fixture()
		{
			loggerMock = mockRepository.Create<ILogger<ReceiveMessageWorker>>();
			receiveControllerMock = mockRepository.Create<IReceiveMessageAdapterController>();
		}

		public ReceiveMessageWorker CreateTestObject()
		{
			return new ReceiveMessageWorker(loggerMock.Object, receiveControllerMock.Object);
		}

		public void Dispose()
		{
			mockRepository.VerifyAll();
		}

		public void PrepareStart()
		{
			receiveControllerMock
				.Setup(x => x.ReceiveAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(0));
		}

		public void PrepareStartWithError()
		{
			receiveControllerMock
				.Setup(x => x.ReceiveAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Throws(() => new InvalidOperationException("Expected Exception during test."));

			SetupLogger(LogLevel.Error, "Exception during receive", e => e.Message == "Expected Exception during test.");
		}

		public void PrepareStartWithCatastrophicError()
		{
			receiveControllerMock
				.Setup(x => x.ReceiveAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Throws(() => new CatastrophicException("Expected Catastrophic Exception during test."));

			SetupLogger(LogLevel.Error, "Catastrophic exception during receive", e => e.Message == "Expected Catastrophic Exception during test.");
		}

		private void SetupLogger(LogLevel expectedLogLevel, string expectedMessage, Func<Exception, bool> validateException)
		{
			Func<object, Type, bool> state = (v, _) => string.Equals(v.ToString(), expectedMessage, StringComparison.OrdinalIgnoreCase);

			loggerMock.Setup(
				x => x.Log(
					It.Is<LogLevel>(l => l == expectedLogLevel),
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => state(v, t)),
					It.Is<Exception>(e => validateException(e)),
					It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
		}

		public void VerifyControllerWasCalled()
		{
			receiveControllerMock
				.Verify(x => x.ReceiveAvailableMessagesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
		}
	}
}