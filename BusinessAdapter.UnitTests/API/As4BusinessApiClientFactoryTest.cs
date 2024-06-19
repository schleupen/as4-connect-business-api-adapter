// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.MP.API;

	[TestFixture]
	internal sealed partial class As4BusinessApiClientFactoryTest : IDisposable
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
		public void CreateAs4BusinessApiClient_WithConfiguration_ShouldCreateClient()
		{
			fixture!.PrepareConfigurationSet();
			As4BusinessApiClientFactory testObject = fixture!.CreateTestObject();

			IAs4BusinessApiClient client = testObject.CreateAs4BusinessApiClient("12345");

			Assert.That(client, Is.Not.Null);
		}

		[Test]
		public void CreateAs4BusinessApiClient_WithoutConfiguration_ShouldThrowCatastrophicException()
		{
			fixture!.PrepareConfigurationNotSet();
			As4BusinessApiClientFactory testObject = fixture!.CreateTestObject();

			CatastrophicException? exception = Assert.Throws<CatastrophicException>(() => testObject.CreateAs4BusinessApiClient("12345"));

			Assert.That(exception!.Message, Contains.Substring("The endpoint for AS4 connect is not configured."));
		}
	}
}
