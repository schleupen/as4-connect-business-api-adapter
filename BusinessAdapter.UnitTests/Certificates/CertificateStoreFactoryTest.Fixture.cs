// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Options;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	internal sealed partial class CertificateStoreFactoryTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IOptions<AdapterOptions>> configurationAccessMock;

			public Fixture()
			{
				configurationAccessMock = mockRepository.Create<IOptions<AdapterOptions>>();
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
					.Setup<AdapterOptions>(x => x.Value).Returns(new AdapterOptions()
					{
						CertificateStoreName = StoreName.My,
						CertificateStoreLocation = StoreLocation.CurrentUser
					});
			}
		}
	}
}