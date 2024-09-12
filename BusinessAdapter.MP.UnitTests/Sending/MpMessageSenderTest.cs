// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	[TestFixture]
	internal sealed partial class MpMessageSenderTest : IDisposable
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
			MpMessageSender testObject = fixture!.CreateTestObject();

			try
			{
				await testObject.SendMessagesAsync(CancellationToken.None);
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
			MpMessageSender testObject = fixture!.CreateTestObject();

			await testObject.SendMessagesAsync(CancellationToken.None);

			fixture.VerifyApiWasNotCalled();
		}

		[Test]
		public async Task SendAvailableMessagesAsync_ShouldCallApiWithOutboxMessageAndRemoveFileAfterwards()
		{
			fixture!.SendAvailableMessagesAsync();
			MpMessageSender testObject = fixture!.CreateTestObject();

			await testObject.SendMessagesAsync(CancellationToken.None);

			// Verify is performed during dispose
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithErrorDuringSend_ShouldThrowExpectedException()
		{
			fixture!.SendAvailableMessagesAsyncWithErrorDuringSend();
			MpMessageSender testObject = fixture!.CreateTestObject();
			try
			{
				await testObject.SendMessagesAsync(CancellationToken.None);
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
			MpMessageSender testObject = fixture!.CreateTestObject();

			await testObject.SendMessagesAsync(CancellationToken.None);

			fixture.VerifyApiWasNotCalled();
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithExceptionDuringApiCreation_ShouldProcessOtherMessages()
		{
			fixture!.SendAvailableMessagesAsyncWithExceptionDuringApiCreation();
			MpMessageSender testObject = fixture!.CreateTestObject();

			try
			{
				await testObject.SendMessagesAsync(CancellationToken.None);
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
			MpMessageSender testObject = fixture!.CreateTestObject();

			await testObject.SendMessagesAsync(CancellationToken.None);

			fixture.VerifyTooManyMessagesErrorWasLogged();
		}

		[Test]
		public async Task SendAvailableMessagesAsync_WithApiExceptionForTooManyMessages_ShouldThrowAggregateException()
		{
			fixture!.SendAvailableMessagesAsyncWithApiException();
			MpMessageSender testObject = fixture!.CreateTestObject();

			try
			{
				await testObject.SendMessagesAsync(CancellationToken.None);
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
			MpMessageSender testObject = fixture!.CreateTestObject();

			await testObject.SendMessagesAsync(CancellationToken.None);

			fixture.VerifySecondMessageWasNotSend();
		}
	}
}
