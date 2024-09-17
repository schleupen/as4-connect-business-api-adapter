// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class BusinessApiGatewayFactoryTest : IDisposable
	{
		private BusinessApiGatewayFactoryTest.Fixture? fixture;

		[SetUp]
		public void Setup()
		{
			fixture = new BusinessApiGatewayFactoryTest.Fixture();
		}

		[TearDown]
		public void Dispose()
		{
			fixture?.Dispose();
			fixture = null;
		}

		[Test]
		public void CreateGateway_WithConfiguration_ShouldCreateClient()
		{
			fixture!.PrepareConfigurationSet();
			BusinessApiGatewayFactory testObject = fixture!.CreateTestObject();

			IBusinessApiGateway gateway = testObject.CreateGateway("12345");

			Assert.That(gateway, Is.Not.Null);
		}

		[Test]
		public void CreateAs4BusinessApiClient_WithoutConfiguration_ShouldThrowCatastrophicException()
		{
			fixture!.PrepareConfigurationNotSet();
			BusinessApiGatewayFactory testObject = fixture!.CreateTestObject();

			CatastrophicException? exception = Assert.Throws<CatastrophicException>(() => testObject.CreateGateway("12345"));

			Assert.That(exception!.Message, Contains.Substring("The endpoint for AS4 connect is not configured."));
		}
	}
}
