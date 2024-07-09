// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.MP;

internal sealed partial class SendMessageWorkerTest
{
	private sealed class Fixture : IDisposable
	{
		private readonly MockRepository mockRepository = new(MockBehavior.Strict);
		private readonly Mock<ILogger<SendMessageWorker>> loggerMock;
		private readonly Mock<ISendMessageAdapterController> sendMessageAdapterControllerMock;
		private readonly Mock<IOptions<SendOptions>> sendOptionsMock;

		public Fixture()
		{
			loggerMock = mockRepository.Create<ILogger<SendMessageWorker>>();
			sendMessageAdapterControllerMock = mockRepository.Create<ISendMessageAdapterController>();
			sendOptionsMock = mockRepository.Create<IOptions<SendOptions>>();
			sendOptionsMock.SetupGet(o => o.Value).Returns(new SendOptions());
		}

		public SendMessageWorker CreateTestObject()
		{
			return new SendMessageWorker(loggerMock.Object, sendMessageAdapterControllerMock.Object, sendOptionsMock.Object);
		}

		public void Dispose()
		{
			mockRepository.VerifyAll();
		}

		public void PrepareStart()
		{
			sendMessageAdapterControllerMock
				.Setup(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(0));
		}

		public void PrepareStartWithError()
		{
			sendMessageAdapterControllerMock
				.Setup(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Throws(() => new InvalidOperationException("Expected Exception during test."));

			SetupLogger(LogLevel.Error, "Error while sending messages", e => e.Message == "Expected Exception during test.");
		}

		public void PrepareStartWithCatastrophicError()
		{
			sendMessageAdapterControllerMock
				.Setup(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()))
				.Throws(() => new CatastrophicException("Expected Catastrophic Exception during test."));

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
			sendMessageAdapterControllerMock.Verify(x => x.SendAvailableMessagesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
		}
	}
}