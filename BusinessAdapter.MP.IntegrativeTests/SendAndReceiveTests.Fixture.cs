namespace Schleupen.AS4.BusinessAdapter.MP;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;
using Schleupen.AS4.BusinessAdapter.MP.Sending;

public partial class SendAndReceiveTests
{
	private Fixture fixture = new();

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
		public TestData Data { get; } = new TestData();

		public void SetupWithMarketpartner(string marketPartner)
		{
			CreateAppSettingsJson(Data.SendDirectory, Data.ReceiveDirectory, new List<string> { marketPartner }, Data.AppSettingsPath);
		}

		public void CreateDirectories()
		{
			Directory.CreateDirectory(Data.SendDirectory);
			Directory.CreateDirectory(Data.ReceiveDirectory);
		}

		public async Task SetupFakeServerAsync()
		{
			await this.fakeServerFixture.ShouldBeHealthyAsync();
			await this.fakeServerFixture.ResetMpMessagesAsync();
		}

		private void CreateAppSettingsJson(
			string sendDirectory,
			string receiveDirectory,
			List<string> marketpartners,
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
			};

			var json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);
			File.WriteAllText(outputPath, json);
		}

		public async Task Receive()
		{
			var serviceProvider = CreateServiceProvider(new FileInfo(Data.AppSettingsPath));
			var sender = serviceProvider.GetRequiredService<IMpMessageReceiver>();

			await sender.ReceiveMessagesAsync(CancellationToken.None);
		}


		public async Task Send()
		{
			var serviceProvider = CreateServiceProvider(new FileInfo(Data.AppSettingsPath));

			var sender = serviceProvider.GetRequiredService<IMpMessageSender>();
			await sender.SendMessagesAsync(CancellationToken.None);
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

		public void VerifySendDirectoryIsEmpty()
		{
			VerifyEmptyDirectory(Data.SendDirectory);
		}

		public void VerifyReceiveDirectoryIsEmpty()
		{
			VerifyEmptyDirectory(Data.ReceiveDirectory);
		}

		public void VerifyEmptyDirectory(string dir)
		{
			var files = Directory.EnumerateFiles(dir).ToList();
			Assert.That(files, Is.Empty, $"files not expected in '{dir}': {string.Join(", ", files)}");
		}

		public void VerifyReceiveDirectoryIsNotEmpty()
		{
			var files = Directory.EnumerateFiles(Data.ReceiveDirectory).ToList();
			Assert.That(files, Is.Not.Empty);
		}

		public void Dispose()
		{
			if(Directory.Exists(Data.SendDirectory)) Directory.Delete(Data.SendDirectory, true);
			if(Directory.Exists(Data.ReceiveDirectory)) Directory.Delete(Data.ReceiveDirectory, true);
		}

		public void VerifySendDirectoryContainsMsconsFile()
		{
			Assert.That(File.Exists(Data.MsconsFileSendFullPath), Is.True);
		}

		public void AddFileFromValidMarketpartnerToSendDirectory()
		{
			File.WriteAllText(Data.MsconsFileSendFullPath, Data.EdifactFileFromValidMarktpartner);
		}

		public void AddFileFromUnkownMarketpartnerToSendDirectory()
		{
			File.WriteAllText(Data.MsconsFileSendFullPath, Data.EdifactFileFromUnknownMarketpartner);
		}
	}

	private sealed class TestData
	{
		private readonly string id = Guid.NewGuid().ToString();
		public string SendDirectory => Path.Combine(Environment.CurrentDirectory, id, "Send");
		public string ReceiveDirectory => Path.Combine(Environment.CurrentDirectory, id, "Receive");
		public string AppSettingsPath => Path.Combine(Environment.CurrentDirectory, id, "appsettings.json");

		public const string MarketpartnerIdWithCertifacte = "9912345000002";
		public const string MarketpartnerIdWithoutCertifacte = "007";
		public const string MsconsFileName = "sample_mscons.edi";

		public string EdifactFileFromValidMarktpartner = $"UNA:+.? '\nUNB+UNOC:3+{MarketpartnerIdWithCertifacte}:500+{MarketpartnerIdWithCertifacte}:500+140204:1109+TSAAAAAMSCONS1++VL++++1'\nUNH+1+MSCONS:D:04B:UN:2.4a'\nBGM+7+MSCONS53945B4603B5EC5AE2FED080C0+9'\nDTM+137:201402041108:203'\nRFF+Z13:13002'\nNAD+MS+9984616000003::293'\nNAD+MR+9912345000002::293'\nUNS+D'\nNAD+DP'\nLOC+172'\nDTM+9:20131231:102'\nRFF+MG:200002'\nCCI+ACH++PMR'\nCCI+16++MRV'\nLIN+1'\nPIA+5+1-1?:1.8.0:SRW'\nQTY+220:7000'\nUNT+17+1'\nUNH+2+MSCONS:D:04B:UN:2.4a'\nBGM+7+MSCONS53945B4603B5EC5AE2FED080C0+9'\nDTM+137:201402041108:203'\nRFF+Z13:13002'\nNAD+MS+9984616000003::293'\nNAD+MR+9912345000002::293'\nUNS+D'\nNAD+DP'\nLOC+172'\nDTM+9:20131231:102'\nRFF+MG:200002'\nCCI+ACH++PMR'\nCCI+16++MRV'\nLIN+1'\nPIA+5+1-1?:1.8.0:SRW'\nQTY+220:7000'\nUNT+17+2'\nUNZ+2+TSAAAAAMSCONS1'\n\n";
		public string EdifactFileFromUnknownMarketpartner = $"UNA:+.? '\nUNB+UNOC:3+{MarketpartnerIdWithoutCertifacte}:500+{MarketpartnerIdWithCertifacte}:500+140204:1109+TSAAAAAMSCONS1++VL++++1'\nUNH+1+MSCONS:D:04B:UN:2.4a'\nBGM+7+MSCONS53945B4603B5EC5AE2FED080C0+9'\nDTM+137:201402041108:203'\nRFF+Z13:13002'\nNAD+MS+9984616000003::293'\nNAD+MR+9912345000002::293'\nUNS+D'\nNAD+DP'\nLOC+172'\nDTM+9:20131231:102'\nRFF+MG:200002'\nCCI+ACH++PMR'\nCCI+16++MRV'\nLIN+1'\nPIA+5+1-1?:1.8.0:SRW'\nQTY+220:7000'\nUNT+17+1'\nUNH+2+MSCONS:D:04B:UN:2.4a'\nBGM+7+MSCONS53945B4603B5EC5AE2FED080C0+9'\nDTM+137:201402041108:203'\nRFF+Z13:13002'\nNAD+MS+9984616000003::293'\nNAD+MR+9912345000002::293'\nUNS+D'\nNAD+DP'\nLOC+172'\nDTM+9:20131231:102'\nRFF+MG:200002'\nCCI+ACH++PMR'\nCCI+16++MRV'\nLIN+1'\nPIA+5+1-1?:1.8.0:SRW'\nQTY+220:7000'\nUNT+17+2'\nUNZ+2+TSAAAAAMSCONS1'\n\n";

		public string MsconsFileSendFullPath => Path.Combine(SendDirectory, MsconsFileName);
	}
}