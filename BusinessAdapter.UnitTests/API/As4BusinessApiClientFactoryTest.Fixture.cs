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

	internal sealed partial class BusinessApiGatewayFactoryTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IOptions<AdapterOptions>> adapterOptions;
			private readonly Mock<IJwtBuilder> jwtHelperMock;
			private readonly Mock<IClientCertificateProvider> marketpartnerCertificateProviderMock;
			private readonly Mock<ILogger<BusinessApiGateway>> clientLoggerMock;
			private readonly Mock<IBusinessApiClientFactory> clientWrapperFactoryMock;
			private readonly Mock<IClientCertificate> certificateMock;
			private readonly X509Certificate2 certificate = new(Array.Empty<byte>());

			public Fixture()
			{
				adapterOptions = mockRepository.Create<IOptions<AdapterOptions>>();
				jwtHelperMock = mockRepository.Create<IJwtBuilder>();
				marketpartnerCertificateProviderMock = mockRepository.Create<IClientCertificateProvider>();
				clientLoggerMock = mockRepository.Create<ILogger<BusinessApiGateway>>();
				clientWrapperFactoryMock = mockRepository.Create<IBusinessApiClientFactory>();
				certificateMock = mockRepository.Create<IClientCertificate>(MockBehavior.Loose);
			}

			public BusinessApiGatewayFactory CreateTestObject()
			{
				return new BusinessApiGatewayFactory(
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
					.Setup(x => x.GetCertificate(It.Is<string>(marketpartnerId => marketpartnerId == "12345")))
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