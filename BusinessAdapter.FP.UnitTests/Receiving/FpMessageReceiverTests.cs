
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Gateways;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Receiving
{
    [TestFixture]
    public class FpMessageReceiverTests
    {
        private Mock<ILogger<FpMessageReceiver>> _loggerMock;
        private IOptions<ReceiveOptions> _receiveOptionsMock;
        private IOptions<AdapterOptions> _adapterOptionsMock;
        private Mock<IBusinessApiGatewayFactory> _businessApiGatewayFactoryMock;
        private Mock<IFpFileRepository> _fpFileRepositoryMock;
        private IOptions<EICMapping> _eicMappingMock;
        private FpMessageReceiver _fpMessageReceiver;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<FpMessageReceiver>>();
            _receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions());
            _adapterOptionsMock = Options.Create<AdapterOptions>(new AdapterOptions());
            _businessApiGatewayFactoryMock = new Mock<IBusinessApiGatewayFactory>();
            _fpFileRepositoryMock = new Mock<IFpFileRepository>();
            _eicMappingMock = Options.Create<EICMapping>(new EICMapping());

            _adapterOptionsMock.Value.Marketpartners = new[] { "9912345000002" };

            List<EICMappingEntry> mappedPartyMock = new List<EICMappingEntry>()
            {
                new EICMappingEntry()
                {
                    Bilanzkreis = "FINGRID",
                    EIC = "asga",
                    FahrplanHaendlertyp = "PPS",
                    MarktpartnerTyp = "BDEW"
                }
            };
            _eicMappingMock.Value.Add("9912345000002", mappedPartyMock);

            _fpMessageReceiver = new FpMessageReceiver(
                _loggerMock.Object,
                _receiveOptionsMock,
                _adapterOptionsMock,
                _businessApiGatewayFactoryMock.Object,
                _fpFileRepositoryMock.Object,
                _eicMappingMock);
        }

        [Test]
        public void ReceiveAvailableMessagesAsync_ThrowsCatastrophicException_WhenReceiveDirectoryIsNotConfigured()
        {
            _receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
            {
                Directory = string.Empty,
                MessageLimitCount = 10,
                Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
            });
            // Act & Assert
            Assert.That(async () => await _fpMessageReceiver.ReceiveAvailableMessagesAsync(CancellationToken.None),
                Throws.TypeOf<CatastrophicException>()
                    .With.Message.EqualTo("The receive directory is not configured."));
        }

        [Test]
        public void ReceiveAvailableMessagesAsync_ThrowsCatastrophicException_WhenNoMarketPartnersAreConfigured()
        {
            _adapterOptionsMock.Value.Marketpartners = new string[0];
            _receiveOptionsMock = Options.Create<ReceiveOptions>(new ReceiveOptions
            {
                Directory = "C:\\Adapter\\Receive",
                MessageLimitCount = 10,
                Retry = new Schleupen.AS4.BusinessAdapter.Configuration.RetryOption { Count = 3 }
            });
            _fpMessageReceiver = new FpMessageReceiver(
                _loggerMock.Object,
                _receiveOptionsMock,
                _adapterOptionsMock,
                _businessApiGatewayFactoryMock.Object,
                _fpFileRepositoryMock.Object,
                _eicMappingMock);

            // Act & Assert
            Assert.That(async () => await _fpMessageReceiver.ReceiveAvailableMessagesAsync(CancellationToken.None),
                Throws.TypeOf<CatastrophicException>()
                    .With.Message.EqualTo("No valid own market partners were found."));
        }
    }
}
