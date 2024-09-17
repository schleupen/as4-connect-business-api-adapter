namespace Schleupen.AS4.BusinessAdapter;

using FP;
using FP.Receiving;
using FP.Sending;
using FP.Configuration;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public partial class SendAndReceiveTests
{
    private Fixture fixture = new Fixture();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        fixture = new Fixture();
        SetupFilesAndDirs();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        fixture?.Dispose();
    }

    private void SetupFilesAndDirs()
    {
	    string testEssFile =
		    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<ScheduleDocument xmlns=\"urn:entsoe.eu:wgedi:ess:scheduledocument:4:1\">\n\t<DocumentIdentification v=\"1234\"/>\n\t<DocumentVersion v=\"1\"/>\n\t<DocumentType v=\"A01\"/>\n\t<ProcessType v=\"A01\"/>\n\t<ScheduleClassificationType v=\"A01\"/>\n\t<SenderIdentification v=\"5790000432752\" codingScheme=\"A10\"/>\n\t<SenderRole v=\"A01\"/>\n\t<ReceiverIdentification v=\"10X000000000RTEM\" codingScheme=\"A01\"/>\n\t<ReceiverRole v=\"A04\"/>\n\t<CreationDateTime v=\"2001-06-02T09:00:00Z\"/>\n\t<ScheduleTimeInterval v=\"2001-06-02T22:00Z/2001-06-03T22:00Z\"/>\n\t<Domain v=\"2Y000002347651H\" codingScheme=\"A01\"/>\n\t<SubjectParty v=\"11X000000100741R\" codingScheme=\"A01\"/>\n\t<SubjectRole v=\"A01\"/>\n\t<MatchingPeriod v=\"2001-06-02T22:00Z/2001-06-03T22:00Z\"/>\n\t<ScheduleTimeSeries>\n\t\t<SendersTimeSeriesIdentification v=\"TS0001\"/>\n\t\t<SendersTimeSeriesVersion v=\"1\"/>\n\t\t<BusinessType v=\"A03\"/>\n\t\t<Product v=\"8716867000016\"/>\n\t\t<ObjectAggregation v=\"A01\"/>\n\t\t<InArea v=\"12Y000002347651H\" codingScheme=\"A01\"/>\n\t\t<OutArea v=\"12YRWENET------Q\" codingScheme=\"A01\"/>\n\t\t<InParty v=\"11X000000100741R\" codingScheme=\"A01\"/>\n\t\t<OutParty v=\"11X000000340533X\" codingScheme=\"A01\"/>\n\t\t<CapacityContractType v=\"A01\"/>\n\t\t<CapacityAgreementIdentification v=\"A1235RT\"/>\n\t\t<MeasurementUnit v=\"MAW\"/>\n\t\t<Period>\n\t\t\t<TimeInterval v=\"2001-06-02T22:00Z/2001-06-03T22:00Z\"/>\n\t\t\t<Resolution v=\"PT15M\"/>\n\t\t\t<Interval>\n\t\t\t\t<Pos v=\"1\"/>\n\t\t\t\t<Qty v=\"45\"/>\n\t\t\t</Interval>\n\t\t</Period>\n\t</ScheduleTimeSeries>\n</ScheduleDocument>";
	    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Receive");
	    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Send");
	    File.WriteAllText(Environment.CurrentDirectory + @"\Send\20010602_PPS_FINGRID_10X000000000RTEM_1234_ACK_.xml", testEssFile);
    }

    private sealed class Fixture : IDisposable
    {

	    public string AppSettingsPath = "./appsettings.Integration.json";
        public Fixture()
        {

		}

        public void CreateDefaultAppSettings()
        {
	        var marketpartners = new List<string> { "9912345000002" };

	        var eicMapping = new Dictionary<string, List<EICMappingEntry>>
	        {
		        {
			        "9912345000002", new List<EICMappingEntry>
			        {
				        new EICMappingEntry
				        {
					        EIC = "5790000432752",
					        MarktpartnerTyp = "BDEW",
					        Bilanzkreis = "FINGRID",
					        FahrplanHaendlerTyp = "PPS"
				        },
				        new EICMappingEntry
				        {
					        EIC = "5790000432766",
					        MarktpartnerTyp = "BDEW",
					        Bilanzkreis = "FINGRID",
					        FahrplanHaendlerTyp = "TPS"
				        },
				        new EICMappingEntry
				        {
					        EIC = "10X000000000RTEM",
					        MarktpartnerTyp = "BDEW",
					        Bilanzkreis = "FINGRID",
					        FahrplanHaendlerTyp = "PPS"
				        }
			        }
		        }
	        };

	        string sendDirectory = Environment.CurrentDirectory + @"\Send";
	        string receiveDirectory =  Environment.CurrentDirectory + @"\Receive";
	        AppSettingsPath = Environment.CurrentDirectory + @"\appsettings.json";

	        CreateAppSettingsJson(sendDirectory, receiveDirectory, marketpartners, eicMapping, AppSettingsPath);
        }

        public void CreateAppSettingsJson(
	        string sendDirectory,
	        string receiveDirectory,
	        List<string> marketpartners,
	        Dictionary<string, List<EICMappingEntry>> eicMapping,
	        string outputPath)
        {
	        var appSettings = new
	        {
		        Adapter = new
		        {
			        As4ConnectEndpoint = "https://localhost:8043",
			        Marketpartners = marketpartners,
			        CertificateStoreLocation = "LocalMachine",
			        CertificateStoreName = "My"
		        },
		        Send = new
		        {
			        Directory = sendDirectory,
			        Retry = new
			        {
				        Count = 3,
				        SleepDuration = "00:00:05"
			        },
			        MessageLimitCount = 2,
			        SleepDuration = "00:00:10"
		        },
		        Receive = new
		        {
			        Directory = receiveDirectory,
			        Retry = new
			        {
				        Count = 1,
				        SleepDuration = "00:00:10"
			        },
			        SleepDuration = "00:01:00",
			        MessageLimitCount = 2
		        },
		        EICMapping = eicMapping
	        };

	        var json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
	        File.WriteAllText(outputPath, json);
        }

        public async Task<IReceiveStatus> Receive(FileInfo configFile)
        {
	        var serviceProvider = CreateServiceProvider(configFile);
	        var sender = serviceProvider.GetRequiredService<IFpMessageReceiver>();

	        return await sender.ReceiveMessagesAsync(CancellationToken.None);
        }


		public async Task<ISendStatus> Send(FileInfo configFile)
        {
	        var serviceProvider = CreateServiceProvider(configFile);

	        var sender = serviceProvider.GetRequiredService<IFpMessageSender>();
	        return await sender.SendMessagesAsync(CancellationToken.None);
        }

        private ServiceProvider CreateServiceProvider(FileInfo fileInfo)
        {
	        var config = new ConfigurationBuilder()
		        .AddJsonFile(fileInfo.FullName, optional: true, reloadOnChange: true)
		        .Build();

	        var serviceCollection = new ServiceCollection();
	        serviceCollection.AddLogging((b) => b.AddConsole());

	        ServiceConfigurator configurator = new ServiceConfigurator();
	        configurator.ConfigureSending(serviceCollection, config);
			configurator.ConfigureReceiving(serviceCollection, config);
			return serviceCollection.BuildServiceProvider();
        }


        public void Dispose()
        {
        }
    }
}