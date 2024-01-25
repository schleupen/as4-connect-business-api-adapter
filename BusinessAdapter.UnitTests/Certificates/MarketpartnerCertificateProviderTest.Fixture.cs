// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using Moq;

	internal sealed partial class MarketpartnerCertificateProviderTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<ICertificateStoreFactory> certificateStoreFactoryMock;
			private readonly Mock<ICertificateStore> certificateStoreMock;
			private readonly Mock<IAs4Certificate> certificate1Mock;
			private readonly Mock<IAs4Certificate> certificate2Mock;

			public Fixture()
			{
				certificateStoreFactoryMock = mockRepository.Create<ICertificateStoreFactory>();
				certificateStoreMock = mockRepository.Create<ICertificateStore>();
				certificate1Mock = mockRepository.Create<IAs4Certificate>();
				certificate2Mock = mockRepository.Create<IAs4Certificate>();
			}

			public MarketpartnerCertificateProvider CreateTestObject()
			{
				return new MarketpartnerCertificateProvider(certificateStoreFactoryMock.Object);
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
					.SetupGet(x => x.As4Certificates)
					.Returns(new List<IAs4Certificate>
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
					.SetupGet(x => x.As4Certificates)
					.Returns(new List<IAs4Certificate>());

				certificateStoreMock
					.Setup(x => x.Dispose());
			}

			public void PrepareCertificateHasNoMatchingNameFound()
			{
				certificateStoreFactoryMock
					.Setup(x => x.CreateAndOpen())
					.Returns(certificateStoreMock.Object);

				certificateStoreMock
					.SetupGet(x => x.As4Certificates)
					.Returns(new List<IAs4Certificate>
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
					.SetupGet(x => x.As4Certificates)
					.Returns(new List<IAs4Certificate>
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
