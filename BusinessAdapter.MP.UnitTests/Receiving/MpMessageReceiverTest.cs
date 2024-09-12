// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	[TestFixture]
	internal sealed partial class MpMessageReceiverTest : IDisposable
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
		public async Task ReceiveAvailableMessagesAsync_ShouldReceiveAllFiles()
		{
			fixture!.PrepareReceiveAvailableMessagesAsync();
			MpMessageReceiver testObject = fixture!.CreateTestObject();

			await testObject.ReceiveMessagesAsync(CancellationToken.None);

			// Verify is performed during dispose
		}
	}
}
