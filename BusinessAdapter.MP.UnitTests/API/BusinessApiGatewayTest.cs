// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using System.Globalization;
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	[TestFixture]
	internal sealed partial class BusinessApiGatewayTest : IDisposable
	{
		private BusinessApiGatewayTest.Fixture? fixture;

		[SetUp]
		public void Setup()
		{
			fixture = new BusinessApiGatewayTest.Fixture();
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

			using BusinessApiGateway testObject = fixture!.CreateTestObject();
			MessageReceiveInfo receiveInfo = await testObject.QueryAvailableMessagesAsync(51);

			Assert.That(receiveInfo.GetAvailableMessages().Length, Is.EqualTo(1));
			MpMessage message = receiveInfo.GetAvailableMessages()[0];
			Assert.That(message.BdewDocumentDate, Is.EqualTo("2024-01-15 15:55:42 +01:00"));
			Assert.That(message.CreatedAt, Is.EqualTo(new DateTimeOffset(new DateTime(2024, 01, 17, 16, 00, 00), TimeSpan.FromHours(1))));
			Assert.That(message.MessageId.ToUpper(CultureInfo.InvariantCulture), Is.EqualTo(fixture.Data.InboxMpMessage.MessageId.ToUpper(CultureInfo.InvariantCulture)));
			Assert.That(message.PartyInfo.Receiver?.Id, Is.EqualTo("ReceiverId"));
			Assert.That(message.PartyInfo.Receiver?.Type, Is.EqualTo("BDEW"));
			Assert.That(message.PartyInfo.Sender?.Id, Is.EqualTo("SenderId"));

			Assert.That(receiveInfo.ConfirmableMessages.Count,Is.EqualTo(0));
			Assert.That(receiveInfo.HasTooManyRequestsError, Is.False);
		}

		[Test]
		public async Task AcknowledgeReceivedMessageAsync_WithSuccessfulCall_ShouldReturnSuccessfulResult()
		{
			InboxMpMessage inboxMpMessage = fixture!.PrepareAcknowledgeReceivedMessage();

			using BusinessApiGateway testObject = fixture!.CreateTestObject();
			BusinessApiResponse<bool> isAcknowledgedResponse = await testObject.AcknowledgeReceivedMessageAsync(inboxMpMessage);

			Assert.That(isAcknowledgedResponse.ApiException, Is.Null);
			Assert.That(isAcknowledgedResponse.WasSuccessful, Is.True);
			Assert.That(isAcknowledgedResponse.Message, Is.True);
			Assert.That(isAcknowledgedResponse.ResponseStatusCode, Is.Null);
		}

		[Test]
		public async Task AcknowledgeReceivedMessageAsync_WithNotSuccessfulCall_ShouldReturnFailed()
		{
			InboxMpMessage inboxMpMessage = fixture!.PrepareAcknowledgeReceivedMessageFailed();
			using BusinessApiGateway testObject = fixture!.CreateTestObject();

			BusinessApiResponse<bool> isAcknowledgedResponse = await testObject.AcknowledgeReceivedMessageAsync(inboxMpMessage);

			Assert.That(isAcknowledgedResponse.ApiException!.Message, Is.EqualTo("something failed\n\nStatus: 409\nResponse: \nThe response"));
			Assert.That(isAcknowledgedResponse.WasSuccessful, Is.False);
			Assert.That(isAcknowledgedResponse.Message, Is.False);
			Assert.That(isAcknowledgedResponse.ResponseStatusCode, Is.EqualTo(HttpStatusCode.Conflict));
		}
	}
}
