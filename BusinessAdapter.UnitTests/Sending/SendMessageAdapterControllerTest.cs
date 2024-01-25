// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.API;

	[TestFixture]
	internal sealed partial class SendMessageAdapterControllerTest : IDisposable
	{
		private Fixture? fixture;

		[SetUp]
		public void Setup()
		{
			fixture = new Fixture();
		}

		[TearDown]
		public void Dispose()
		{
			fixture?.Dispose();
			fixture = null;
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithoutSendDirectory_ShouldThrowCatastrophicException()
		{
			fixture!.SendAvailableMessagesAsyncWithoutSendDirectory();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			try
			{
				await testObject.SendAvailableMessagesAsync(CancellationToken.None);
			}
			catch (CatastrophicException exception)
			{
				Assert.That(exception.Message, Contains.Substring("The send directory is not configured."));
				Assert.Pass();
				return;
			}

			Assert.Fail("Expected an exception of type CatastrophicException but did not receive any.");
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithoutEdifactFiles_ShouldNeverCallApi()
		{
			fixture!.SendAvailableMessagesAsyncWithoutEdifactFiles();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			await testObject.SendAvailableMessagesAsync(CancellationToken.None);

			fixture.VerifyApiWasNotCalled();
		}

		[Test]
		public async Task SendAvailableMessagesAsync_ShouldCallApiWithOutboxMessageAndRemoveFileAfterwards()
		{
			fixture!.SendAvailableMessagesAsync();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			await testObject.SendAvailableMessagesAsync(CancellationToken.None);

			// Verify is performed during dispose
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithErrorDuringSend_ShouldThrowExpectedException()
		{
			fixture!.SendAvailableMessagesAsyncWithErrorDuringSend();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();
			try
			{
				await testObject.SendAvailableMessagesAsync(CancellationToken.None);
			}
			catch (AggregateException e) when (e.InnerExceptions[0].Message == "Expected")
			{
				Assert.Pass();
				return;
			}

			Assert.Fail("Expected an exception but did not receive one.");
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithSenderIdentificationNumberNotResolvable_ShouldNotCallApi()
		{
			fixture!.SendAvailableMessagesAsyncWithSenderIdentificationNumberNotResolvable();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			await testObject.SendAvailableMessagesAsync(CancellationToken.None);

			fixture.VerifyApiWasNotCalled();
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithExceptionDuringApiCreation_ShouldProcessOtherMessages()
		{
			fixture!.SendAvailableMessagesAsyncWithExceptionDuringApiCreation();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			try
			{
				await testObject.SendAvailableMessagesAsync(CancellationToken.None);
			}
			catch (AggregateException e) when (e.InnerExceptions[0].Message == "Expected during API creation")
			{
				Assert.Pass();
				return;
			}

			Assert.Fail("Expected an exception but did not receive one.");
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithApiExceptionForTooManyMessages_ShouldLogTooManyMessagesError()
		{
			fixture!.SendAvailableMessagesAsyncWithApiExceptionForTooManyMessages();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			await testObject.SendAvailableMessagesAsync(CancellationToken.None);

			fixture.VerifyTooManyMessagesErrorWasLogged();
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithApiExceptionForTooManyMessages_ShouldThrowAggregateException()
		{
			fixture!.SendAvailableMessagesAsyncWithApiException();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			try
			{
				await testObject.SendAvailableMessagesAsync(CancellationToken.None);
			}
			catch (AggregateException e)
			{
				string message = e.InnerExceptions[0].Message;
				Assert.That(message, Contains.Substring("Error from API"));
				Assert.That(message, Contains.Substring("Status: 502"));
				Assert.Pass();
			}

			Assert.Fail("Expected exception but did not receive one.");
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithMoreMessagesThanConfiguredLimit_ShouldOnlySendMessagesUpToLimit()
		{
			fixture!.SendAvailableMessagesAsyncWithMoreMessagesThanLimit();
			SendMessageAdapterController testObject = fixture!.CreateTestObject();

			await testObject.SendAvailableMessagesAsync(CancellationToken.None);

			fixture.VerifySecondMessageWasNotSend();
		}
	}
}
