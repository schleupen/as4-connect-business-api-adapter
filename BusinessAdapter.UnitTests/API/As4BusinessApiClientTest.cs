// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System.Net;
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.Receiving;

	[TestFixture]
	internal sealed partial class As4BusinessApiClientTest : IDisposable
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
		public async Task QueryAvailableMessagesAsync_ShouldGetCertificateAndCallClient()
		{
			fixture!.PrepareQueryAvailableMessages();
			using (As4BusinessApiClient testObject = fixture!.CreateTestObject())
			{
				MessageReceiveInfo receiveInfo = await testObject.QueryAvailableMessagesAsync(51);

				fixture.ValidateReceiveInfo(receiveInfo);
			}
		}

		[Test]
		public async Task AcknowledgeReceivedMessageAsync_WithSuccessfulCall_ShouldReturnSuccessfulResult()
		{
			InboxMessage inboxMessage = fixture!.PrepareAcknowledgeReceivedMessage();
			using (As4BusinessApiClient testObject = fixture!.CreateTestObject())
			{
				MessageResponse<bool> isAcknowledgedResponse = await testObject.AcknowledgeReceivedMessageAsync(inboxMessage);

				Assert.That(isAcknowledgedResponse.ApiException, Is.Null);
				Assert.That(isAcknowledgedResponse.WasSuccessful, Is.True);
				Assert.That(isAcknowledgedResponse.Payload, Is.True);
				Assert.That(isAcknowledgedResponse.ResponseStatusCode, Is.Null);
			}
		}

		[Test]
		public async Task AcknowledgeReceivedMessageAsync_WithNotSuccessfulCall_ShouldReturnFailed()
		{
			InboxMessage inboxMessage = fixture!.PrepareAcknowledgeReceivedMessageFailed();
			using (As4BusinessApiClient testObject = fixture!.CreateTestObject())
			{
				MessageResponse<bool> isAcknowledgedResponse = await testObject.AcknowledgeReceivedMessageAsync(inboxMessage);

				Assert.That(isAcknowledgedResponse.ApiException!.Message, Is.EqualTo("something failed\n\nStatus: 409\nResponse: \nThe response"));
				Assert.That(isAcknowledgedResponse.WasSuccessful, Is.False);
				Assert.That(isAcknowledgedResponse.Payload, Is.False);
				Assert.That(isAcknowledgedResponse.ResponseStatusCode, Is.EqualTo(HttpStatusCode.Conflict));
			}
		}
	}
}
