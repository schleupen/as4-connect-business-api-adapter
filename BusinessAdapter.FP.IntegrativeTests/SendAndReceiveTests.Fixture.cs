namespace Schleupen.AS4.BusinessAdapter.FP;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Sending;

public partial class SendAndReceiveTests
{
	private Fixture fixture = new();

	public void Dispose()
	{
		this.fixture?.Dispose();
	}

	[SetUp]
	public void Setup()
	{
		fixture = new Fixture();
		fixture.CreateDirectories();
		fixture.SetupFakeServerAsync().Wait();
	}

	[TearDown]
	public void TearDown()
	{
		fixture?.Dispose();
	}

	private sealed class Fixture : IDisposable
	{
		private readonly FakeServerFixture fakeServerFixture = new();

		public readonly TestData Data = new TestData();

		public async Task SetupFakeServerAsync()
		{
			await this.fakeServerFixture.ShouldBeHealthyAsync();
			await this.fakeServerFixture.ResetFpMessagesAsync();
		}

		public void AddFileToSendDirectory()
		{
			File.WriteAllText(Data.FpFileSendFullPath, TestData.FpEssFileContent);
		}

		public void CreateDirectories()
		{
			Directory.CreateDirectory(Data.SendDirectory);
			Directory.CreateDirectory(Data.ReceiveDirectory);
		}

		public void SetupWithMarketpartner(string marketPartner)
		{
			var marketPartners = new List<string> { marketPartner };
			var eicMapping = CreateEicMapping();

			var appSettings = new
			{
				Adapter = new
				{
					As4ConnectEndpoint = "https://localhost:8043",
					Marketpartners = marketPartners,
					CertificateStoreLocation = "LocalMachine",
					CertificateStoreName = "My"
				},
				Send = new
				{
					Directory = Data.SendDirectory,
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
					Directory = Data.ReceiveDirectory,
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
			File.WriteAllText(Data.AppSettingsPath, json);
		}

		private static Dictionary<string, List<EICMappingEntry>> CreateEicMapping()
		{
			var eicMapping = new Dictionary<string, List<EICMappingEntry>>
			{
				{
					TestData.MarketpartnerIdWithCertificate, new List<EICMappingEntry>
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
				},
				{
					"9912345000003", new List<EICMappingEntry>
					{
						new EICMappingEntry
						{
							EIC = "5790000432755",
							MarktpartnerTyp = "BDEW",
							Bilanzkreis = "FINGRID",
							FahrplanHaendlerTyp = "PPS"
						}
					}
				}
			};
			return eicMapping;
		}

		public void VerifySendDirectoryIsEmpty()
		{
			VerifyEmptyDirectory(Data.SendDirectory);
		}

		public void VerifyReceiveDirectoryIsEmpty()
		{
			VerifyEmptyDirectory(Data.ReceiveDirectory);
		}

		public void VerifyReceiveDirectoryIsNotEmpty()
		{
			var files = Directory.EnumerateFiles(Data.ReceiveDirectory).ToList();
			Assert.That(files, Is.Not.Empty);
		}

		public void VerifyEmptyDirectory(string dir)
		{
			var files = Directory.EnumerateFiles(dir).ToList();
			Assert.That(files, Is.Empty, $"files not expected in '{dir}': {string.Join(", ", files)}");
		}

		public async Task<IReceiveStatus> Receive()
		{
			var serviceProvider = CreateServiceProvider(new FileInfo(this.Data.AppSettingsPath));
			var sender = serviceProvider.GetRequiredService<IFpMessageReceiver>();

			return await sender.ReceiveMessagesAsync(CancellationToken.None);
		}


		public async Task<ISendStatus> Send()
		{
			var serviceProvider = CreateServiceProvider(new FileInfo(this.Data.AppSettingsPath));

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

		private void DeleteDirectories()
		{
			if (Directory.Exists(Data.SendDirectory)) Directory.Delete(Data.SendDirectory, true);
			if (Directory.Exists(Data.ReceiveDirectory)) Directory.Delete(Data.ReceiveDirectory, true);
		}

		public void Dispose()
		{
			this.DeleteDirectories();
		}
	}

	private sealed class TestData
	{
		private readonly string id = Guid.NewGuid().ToString();
		public string SendDirectory => Path.Combine(Environment.CurrentDirectory, id, "Send");
		public string ReceiveDirectory => Path.Combine(Environment.CurrentDirectory, id, "Receive");
		public string AppSettingsPath => Path.Combine(Environment.CurrentDirectory, id, "appsettings.json");

		public const string MarketpartnerIdWithCertificate = "9912345000002";
		public const string MarketpartnerIdWithoutCertificate = "9912345000003";
		public const string MarketpartnerIdWithoutMapping = "9912345000010";
		public const string FpFileName = "20010602_PPS_10X000000000RTEM_FINGRID_1234_ACK_.xml";
		public string FpFileSendFullPath => Path.Combine(SendDirectory, FpFileName);

		public const string FpEssFileContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<ScheduleDocument xmlns=\"urn:entsoe.eu:wgedi:ess:scheduledocument:4:1\">\n\t<DocumentIdentification v=\"1234\"/>\n\t<DocumentVersion v=\"1\"/>\n\t<DocumentType v=\"A01\"/>\n\t<ProcessType v=\"A01\"/>\n\t<ScheduleClassificationType v=\"A01\"/>\n\t<SenderIdentification v=\"10X000000000RTEM\" codingScheme=\"A10\"/>\n\t<SenderRole v=\"A01\"/>\n\t<ReceiverIdentification v=\"10X000000000RTEM\" codingScheme=\"A01\"/>\n\t<ReceiverRole v=\"A04\"/>\n\t<CreationDateTime v=\"2001-06-02T09:00:00Z\"/>\n\t<ScheduleTimeInterval v=\"2001-06-02T22:00Z/2001-06-03T22:00Z\"/>\n\t<Domain v=\"2Y000002347651H\" codingScheme=\"A01\"/>\n\t<SubjectParty v=\"11X000000100741R\" codingScheme=\"A01\"/>\n\t<SubjectRole v=\"A01\"/>\n\t<MatchingPeriod v=\"2001-06-02T22:00Z/2001-06-03T22:00Z\"/>\n\t<ScheduleTimeSeries>\n\t\t<SendersTimeSeriesIdentification v=\"TS0001\"/>\n\t\t<SendersTimeSeriesVersion v=\"1\"/>\n\t\t<BusinessType v=\"A03\"/>\n\t\t<Product v=\"8716867000016\"/>\n\t\t<ObjectAggregation v=\"A01\"/>\n\t\t<InArea v=\"12Y000002347651H\" codingScheme=\"A01\"/>\n\t\t<OutArea v=\"12YRWENET------Q\" codingScheme=\"A01\"/>\n\t\t<InParty v=\"11X000000100741R\" codingScheme=\"A01\"/>\n\t\t<OutParty v=\"11X000000340533X\" codingScheme=\"A01\"/>\n\t\t<CapacityContractType v=\"A01\"/>\n\t\t<CapacityAgreementIdentification v=\"A1235RT\"/>\n\t\t<MeasurementUnit v=\"MAW\"/>\n\t\t<Period>\n\t\t\t<TimeInterval v=\"2001-06-02T22:00Z/2001-06-03T22:00Z\"/>\n\t\t\t<Resolution v=\"PT15M\"/>\n\t\t\t<Interval>\n\t\t\t\t<Pos v=\"1\"/>\n\t\t\t\t<Qty v=\"45\"/>\n\t\t\t</Interval>\n\t\t</Period>\n\t</ScheduleTimeSeries>\n</ScheduleDocument>";
	}
}