// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class ReceiveMessageAdapterControllerTest : IDisposable
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
			ReceiveMessageAdapterController testObject = fixture!.CreateTestObject();

			await testObject.ReceiveAvailableMessagesAsync(CancellationToken.None);

			// Verify is performed during dispose
		}
	}
}
