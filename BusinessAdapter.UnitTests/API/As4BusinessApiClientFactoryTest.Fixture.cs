// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.MP.API;

	internal sealed partial class As4BusinessApiClientFactoryTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IOptions<AdapterOptions>> adapterOptions;
			private readonly Mock<IJwtHelper> jwtHelperMock;
			private readonly Mock<IMarketpartnerCertificateProvider> marketpartnerCertificateProviderMock;
			private readonly Mock<ILogger<As4BusinessApiClient>> clientLoggerMock;
			private readonly Mock<IClientWrapperFactory> clientWrapperFactoryMock;
			private readonly Mock<IAs4Certificate> certificateMock;
			private readonly X509Certificate2 certificate = new(Array.Empty<byte>());

			public Fixture()
			{
				adapterOptions = mockRepository.Create<IOptions<AdapterOptions>>();
				jwtHelperMock = mockRepository.Create<IJwtHelper>();
				marketpartnerCertificateProviderMock = mockRepository.Create<IMarketpartnerCertificateProvider>();
				clientLoggerMock = mockRepository.Create<ILogger<As4BusinessApiClient>>();
				clientWrapperFactoryMock = mockRepository.Create<IClientWrapperFactory>();
				certificateMock = mockRepository.Create<IAs4Certificate>(MockBehavior.Loose);
			}

			public As4BusinessApiClientFactory CreateTestObject()
			{
				return new As4BusinessApiClientFactory(
					adapterOptions.Object,
					jwtHelperMock.Object,
					marketpartnerCertificateProviderMock.Object,
					clientLoggerMock.Object,
					clientWrapperFactoryMock.Object);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
				certificate.Dispose();
			}

			public void PrepareConfigurationSet()
			{
				SetupEndpoint("https://test123");
				SetupCertificateProvider();
			}

			private void SetupCertificateProvider()
			{
				marketpartnerCertificateProviderMock
					.Setup(x => x.GetMarketpartnerCertificate(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
					.Returns(certificateMock.Object);

				certificateMock
					.Setup(x => x.AsX509Certificate())
					.Returns(certificate);
			}

			public void PrepareConfigurationNotSet()
			{
				SetupEndpoint(string.Empty);
			}

			private void SetupEndpoint(string endpoint)
			{
				adapterOptions
					.Setup(x => x.Value)
					.Returns(new AdapterOptions() { As4ConnectEndpoint = endpoint });
			}
		}
	}
}