// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class CertificateStoreFactoryTest : IDisposable
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
		public void CreateAndOpen_ShouldCallConfiguraitonAndCreateStore()
		{
			fixture!.PrepareCreateAndOpen();
			CertificateStoreFactory testObject = fixture!.CreateTestObject();

			using (IClientCertificateStore result = testObject.CreateAndOpen())
			{
				Assert.That(result, Is.Not.Null);
			}
		}
	}
}
