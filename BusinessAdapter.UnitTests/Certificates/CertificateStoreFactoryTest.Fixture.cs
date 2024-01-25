// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	internal sealed partial class CertificateStoreFactoryTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IConfigurationAccess> configurationAccessMock;

			public Fixture()
			{
				configurationAccessMock = mockRepository.Create<IConfigurationAccess>();
			}

			public CertificateStoreFactory CreateTestObject()
			{
				return new CertificateStoreFactory(configurationAccessMock.Object);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
			}

			public void PrepareCreateAndOpen()
			{
				configurationAccessMock
					.Setup(x => x.GetCertificateStoreLocation())
					.Returns(StoreLocation.CurrentUser);

				configurationAccessMock
					.Setup(x => x.GetCertificateStoreName())
					.Returns(StoreName.My);
			}
		}
	}
}
