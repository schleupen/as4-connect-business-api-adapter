// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using Moq;

	internal sealed partial class ClientCertificateProviderTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<ICertificateStoreFactory> certificateStoreFactoryMock;
			private readonly Mock<IClientCertificateStore> certificateStoreMock;
			private readonly Mock<IClientCertificate> certificate1Mock;
			private readonly Mock<IClientCertificate> certificate2Mock;

			public Fixture()
			{
				certificateStoreFactoryMock = mockRepository.Create<ICertificateStoreFactory>();
				certificateStoreMock = mockRepository.Create<IClientCertificateStore>();
				certificate1Mock = mockRepository.Create<IClientCertificate>();
				certificate2Mock = mockRepository.Create<IClientCertificate>();
			}

			public ClientCertificateProvider CreateTestObject()
			{
				return new ClientCertificateProvider(certificateStoreFactoryMock.Object);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
			}

			public void PrepareCertificateFound()
			{
				certificateStoreFactoryMock
					.Setup(x => x.CreateAndOpen())
					.Returns(certificateStoreMock.Object);

				certificateStoreMock
					.SetupGet(x => x.Certificates)
					.Returns(new List<IClientCertificate>
							{
								certificate1Mock.Object
							});

				certificateStoreMock
					.Setup(x => x.Dispose());

				certificate1Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == "12345")))
					.Returns(true);
			}

			public void PrepareNoAs4CertificateFound()
			{
				certificateStoreFactoryMock
					.Setup(x => x.CreateAndOpen())
					.Returns(certificateStoreMock.Object);

				certificateStoreMock
					.SetupGet(x => x.Certificates)
					.Returns(new List<IClientCertificate>());

				certificateStoreMock
					.Setup(x => x.Dispose());
			}

			public void PrepareCertificateHasNoMatchingNameFound()
			{
				certificateStoreFactoryMock
					.Setup(x => x.CreateAndOpen())
					.Returns(certificateStoreMock.Object);

				certificateStoreMock
					.SetupGet(x => x.Certificates)
					.Returns(new List<IClientCertificate>
							{
								certificate1Mock.Object
							});

				certificateStoreMock
					.Setup(x => x.Dispose());

				certificate1Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == "12345")))
					.Returns(false);
			}

			public void PrepareMultipleCertificatesFound()
			{
				certificateStoreFactoryMock
					.Setup(x => x.CreateAndOpen())
					.Returns(certificateStoreMock.Object);

				certificateStoreMock
					.SetupGet(x => x.Certificates)
					.Returns(new List<IClientCertificate>
							{
								certificate1Mock.Object,
								certificate2Mock.Object
							});

				certificateStoreMock
					.Setup(x => x.Dispose());

				certificate1Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == "12345")))
					.Returns(true);

				certificate2Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == "12345")))
					.Returns(true);
			}
		}
	}
}
