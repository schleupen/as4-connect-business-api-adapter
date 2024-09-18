namespace Schleupen.AS4.BusinessAdapter;

using Schleupen.AS4.BusinessAdapter.MP;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;
using Schleupen.AS4.BusinessAdapter.MP.Sending;
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
	    string testEdifactFile =
		    "UNA:+.? '\nUNB+UNOC:3+9984616000003:500+9912345000002:500+140204:1109+TSAAAAAMSCONS1++VL++++1'\nUNH+1+MSCONS:D:04B:UN:2.4a'\nBGM+7+MSCONS53945B4603B5EC5AE2FED080C0+9'\nDTM+137:201402041108:203'\nRFF+Z13:13002'\nNAD+MS+9984616000003::293'\nNAD+MR+9912345000002::293'\nUNS+D'\nNAD+DP'\nLOC+172'\nDTM+9:20131231:102'\nRFF+MG:200002'\nCCI+ACH++PMR'\nCCI+16++MRV'\nLIN+1'\nPIA+5+1-1?:1.8.0:SRW'\nQTY+220:7000'\nUNT+17+1'\nUNH+2+MSCONS:D:04B:UN:2.4a'\nBGM+7+MSCONS53945B4603B5EC5AE2FED080C0+9'\nDTM+137:201402041108:203'\nRFF+Z13:13002'\nNAD+MS+9984616000003::293'\nNAD+MR+9912345000002::293'\nUNS+D'\nNAD+DP'\nLOC+172'\nDTM+9:20131231:102'\nRFF+MG:200002'\nCCI+ACH++PMR'\nCCI+16++MRV'\nLIN+1'\nPIA+5+1-1?:1.8.0:SRW'\nQTY+220:7000'\nUNT+17+2'\nUNZ+2+TSAAAAAMSCONS1'\n\n";
	    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Receive");
	    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Send");
	    File.WriteAllText(Environment.CurrentDirectory + @"\Send\sample_mscons.edi", testEdifactFile);
    }

    private sealed class Fixture : IDisposable
    {

	    public string AppSettingsPath = "./appsettings.Integration.json";
        public Fixture()
        {

		}

        public void CreateDefaultAppSettings(string marketPartner = "9912345000002")
        {
	        var marketpartners = new List<string> {  marketPartner };
	        
	        string sendDirectory = Environment.CurrentDirectory + @"\Send";
	        string receiveDirectory =  Environment.CurrentDirectory + @"\Receive";
	        AppSettingsPath = Environment.CurrentDirectory + @"\appsettings.json";

	        CreateAppSettingsJson(sendDirectory, receiveDirectory, marketpartners, AppSettingsPath);
        }

        public bool CheckSendFile()
        {
	        var fileName = "sample_mscons.edi";
	        var directory = Environment.CurrentDirectory + @"\Send";
	        return File.Exists(Path.Combine(directory, fileName));
        }

        public bool CheckReceiveFileDirIsNotEmpty()
        {
	        var directory = Environment.CurrentDirectory + @"\Send";
	        return !Directory.EnumerateFileSystemEntries(directory).Any();
        }

        public void CreateAppSettingsJson(
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

        public async Task Receive(FileInfo configFile)
        {
	        var serviceProvider = CreateServiceProvider(configFile);
	        var sender = serviceProvider.GetRequiredService<IMpMessageReceiver>();

	        await sender.ReceiveMessagesAsync(CancellationToken.None);
        }


		public async Task Send(FileInfo configFile)
        {
	        var serviceProvider = CreateServiceProvider(configFile);

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
        
        public void Dispose()
        {
        }
    }
}