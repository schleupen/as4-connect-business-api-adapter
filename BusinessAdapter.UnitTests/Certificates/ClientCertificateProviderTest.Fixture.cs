// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using Microsoft.Extensions.Logging;
	using Moq;
	using NUnit.Framework;

	internal sealed partial class ClientCertificateProviderTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<ICertificateStoreFactory> certificateStoreFactoryMock;
			private readonly Mock<IClientCertificateStore> certificateStoreMock;
			private readonly Mock<IClientCertificate> certificate1Mock;
			private readonly Mock<IClientCertificate> certificate2Mock;
			private readonly Mock<ILogger<ClientCertificateProvider>> loggerMock;
			public string MarktPartnerId { get; } = "12345";

			public Fixture()
			{
				certificateStoreFactoryMock = mockRepository.Create<ICertificateStoreFactory>();
				certificateStoreMock = mockRepository.Create<IClientCertificateStore>();
				certificate1Mock = mockRepository.Create<IClientCertificate>();
				certificate2Mock = mockRepository.Create<IClientCertificate>();
				loggerMock = mockRepository.Create<ILogger<ClientCertificateProvider>>();
			}

			public ClientCertificateProvider CreateTestObject()
			{
				return new ClientCertificateProvider(certificateStoreFactoryMock.Object, loggerMock.Object);
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
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);

				certificate1Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now - TimeSpan.FromDays(2));
				certificate1Mock
					.Setup(x => x.ValidUntil)
					.Returns(DateTime.Now + TimeSpan.FromDays(2));

				loggerMock.Setup(logger => logger.Log(
					It.IsAny<LogLevel>(),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!));

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
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(false);
			}

			public void IsCertificateOne(IClientCertificate certificate)
			{
				Assert.That(certificate, Is.EqualTo(certificate1Mock.Object));
			}

			public void IsCertificateTwo(IClientCertificate certificate)
			{
				Assert.That(certificate, Is.EqualTo(certificate2Mock.Object));
			}

			public void PrepareMultipleValidCertificatesFound()
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

				//valid cert
				certificate1Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);
				certificate1Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now - TimeSpan.FromDays(2));
				certificate1Mock
					.Setup(x => x.ValidUntil)
					.Returns(DateTime.Now + TimeSpan.FromDays(1));

				//invalid cert
				certificate2Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);
				certificate2Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now - TimeSpan.FromDays(1));
				certificate2Mock
					.Setup(x => x.ValidUntil)
					.Returns(DateTime.Now + TimeSpan.FromDays(2));

				loggerMock.Setup(logger => logger.Log(
					It.IsAny<LogLevel>(),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!));
			}

			public void PrepareMultipleCertificatesWithOneValidCertificateFound()
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

				//valid Cert
				certificate1Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);
				certificate1Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now - TimeSpan.FromDays(2));
				certificate1Mock
					.Setup(x => x.ValidUntil)
					.Returns(DateTime.Now + TimeSpan.FromDays(1));

				//invalid Cert
				certificate2Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);
				certificate2Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now + TimeSpan.FromDays(2));

				loggerMock.Setup(logger => logger.Log(
					It.IsAny<LogLevel>(),
					It.IsAny<EventId>(),
					It.IsAny<It.IsAnyType>(),
					It.IsAny<Exception>(),
					((Func<It.IsAnyType, Exception, string>)It.IsAny<object>())!));
			}

			public void PrepareMultipleCertificatesWithNoValidCertificatesFound()
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

				//invalid cert
				certificate1Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);
				certificate1Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now - TimeSpan.FromDays(2));
				certificate1Mock
					.Setup(x => x.ValidUntil)
					.Returns(DateTime.Now - TimeSpan.FromDays(1));

				//invalid cert
				certificate2Mock
					.Setup(x => x.IsCertificateFor(It.Is<string>(id => id == MarktPartnerId)))
					.Returns(true);
				certificate2Mock
					.Setup(x => x.ValidFrom)
					.Returns(DateTime.Now + TimeSpan.FromDays(2));
			}
		}
	}
}
