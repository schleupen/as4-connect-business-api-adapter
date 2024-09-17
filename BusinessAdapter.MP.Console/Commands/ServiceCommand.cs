namespace Schleupen.AS4.BusinessAdapter.MP.Commands;

using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class ServiceCommand : Command
{
	public ServiceCommand() : base("service", "send and receives messages continuously")
	{
		var configFileOption = new ConfigFileOption();
		this.AddOption(configFileOption);
		this.SetHandler(RunService, configFileOption);
	}

	private async Task RunService(FileInfo configFile)
	{
		var serviceHost = this.BuildServiceHost(configFile);

		await serviceHost.RunAsync();
	}

	private IHost BuildServiceHost(FileInfo configFile)
	{
		HostApplicationBuilder builder = Host.CreateApplicationBuilder(Array.Empty<string>());
		ServiceConfigurator configurator = new ServiceConfigurator();
		builder.Configuration.AddJsonFile(configFile.FullName);
		configurator.ConfigureService(builder.Services, builder.Configuration);

		IHost host = builder.Build();
		return host;
	}
}