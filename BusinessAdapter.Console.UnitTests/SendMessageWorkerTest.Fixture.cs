// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Microsoft.Extensions.Logging;
using Moq;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Sending;

internal sealed partial class SendMessageWorkerTest
{
	private sealed class Fixture : IDisposable
	{
		private readonly MockRepository mockRepository = new(MockBehavior.Strict);
		private readonly Mock<ILogger<SendMpMessageWorker>> loggerMock;
		private readonly Mock<ISendMessageAdapterController> receiveControllerMock;
		private readonly Mock<ISendMessageAdapterControllerFactory> receiveControllerFactoryMock;

		public Fixture()
		{
			loggerMock = mockRepository.Create<ILogger<SendMpMessageWorker>>();
			receiveControllerMock = mockRepository.Create<ISendMessageAdapterController>();
			receiveControllerFactoryMock = mockRepository.Create<ISendMessageAdapterControllerFactory>();
		}

		public SendMpMessageWorker CreateTestObject()
		{
			return new SendMpMessageWorker(loggerMock.Object, receiveControllerFactoryMock.Object);
		}

		public void Dispose()
		{
			mockRepository.VerifyAll();
		}

		public void PrepareStart()
		{
			receiveControllerMock
				.Setup(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(0));

			receiveControllerFactoryMock
				.Setup(f => f.GetSendController(ControllerType.MP))
				.Returns(receiveControllerMock.Object);
		}

		public void PrepareStartWithError()
		{
			receiveControllerMock
				.Setup(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Throws(() => new InvalidOperationException("Expected Exception during test."));

			receiveControllerFactoryMock
				.Setup(f => f.GetSendController(ControllerType.MP))
				.Returns(receiveControllerMock.Object);
						
			SetupLogger(LogLevel.Error, "Error while sending messages", e => e.Message == "Expected Exception during test.");
		}

		public void PrepareStartWithCatastrophicError()
		{
			receiveControllerMock
				.Setup(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Throws(() => new CatastrophicException("Expected Catastrophic Exception during test."));

			receiveControllerFactoryMock
				.Setup(f => f.GetSendController(ControllerType.MP))
				.Returns(receiveControllerMock.Object);

			SetupLogger(LogLevel.Error, "Catastrophic exception while sending", e => e.Message == "Expected Catastrophic Exception during test.");
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
				.Verify(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
		}
	}
}