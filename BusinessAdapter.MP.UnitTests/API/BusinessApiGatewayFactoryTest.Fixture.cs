// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.API
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Moq;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.API.Assemblers;
	using Schleupen.AS4.BusinessAdapter.Certificates;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	internal sealed partial class BusinessApiGatewayFactoryTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly MockRepository mockRepository = new(MockBehavior.Strict);
			private readonly Mock<IOptions<AdapterOptions>> adapterOptions;
			private readonly Mock<IJwtBuilder> jwtHelperMock;
			private readonly Mock<ILogger<BusinessApiGateway>> clientLoggerMock;
			private readonly Mock<IBusinessApiClientFactory> clientWrapperFactoryMock;
			private readonly Mock<IClientCertificate> certificateMock;
			private readonly Mock<IPartyIdTypeAssembler> partyIdTypeAssembler;
			private readonly Mock<IHttpClientFactory> httpClientFactory;
			private readonly X509Certificate2 certificate = new(Array.Empty<byte>());


			public Fixture()
			{
				adapterOptions = mockRepository.Create<IOptions<AdapterOptions>>();
				jwtHelperMock = mockRepository.Create<IJwtBuilder>();
				clientLoggerMock = mockRepository.Create<ILogger<BusinessApiGateway>>();
				clientWrapperFactoryMock = mockRepository.Create<IBusinessApiClientFactory>();
				certificateMock = mockRepository.Create<IClientCertificate>(MockBehavior.Loose);
				partyIdTypeAssembler = mockRepository.Create<IPartyIdTypeAssembler>(MockBehavior.Loose);
				httpClientFactory = mockRepository.Create<IHttpClientFactory>(MockBehavior.Loose);
			}

			public BusinessApiGatewayFactory CreateTestObject()
			{
				return new BusinessApiGatewayFactory(
					adapterOptions.Object,
					jwtHelperMock.Object,
					clientLoggerMock.Object,
					clientWrapperFactoryMock.Object,
					partyIdTypeAssembler.Object,
					httpClientFactory.Object);
			}

			public void Dispose()
			{
				mockRepository.VerifyAll();
				certificate.Dispose();
			}

			public void PrepareConfigurationSet()
			{
				SetupEndpoint("https://test123");
				SetupHttpClientFactory();
			}

			private void SetupHttpClientFactory()
			{
				httpClientFactory
					.Setup(x => x.CreateFor(It.Is<Party>(party => party.Id == "12345")))
#pragma warning disable CA2000
					.Returns(new HttpClient());
#pragma warning restore CA2000
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