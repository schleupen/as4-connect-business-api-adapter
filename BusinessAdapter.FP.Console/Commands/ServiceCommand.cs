namespace Schleupen.AS4.BusinessAdapter.FP.Commands;

using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class ServiceCommand : Command
{
	public ServiceCommand() : base("service", "send and receives messages continuously")
	{
		var configFileOption = new ConfigFileOption();

		Options.Add(configFileOption);

		SetAction((parseResult, cancellationToken) =>
		{
			var configFile = parseResult.GetValue(configFileOption);

			return RunService(configFile!, cancellationToken);
		});
	}

	private Task RunService(FileInfo configFile, CancellationToken cancellationToken)
	{
		var serviceHost = BuildServiceHost(configFile);

		return serviceHost.RunAsync(cancellationToken);
	}

	private IHost BuildServiceHost(FileInfo configFile)
	{
		HostApplicationBuilder builder = Host.CreateApplicationBuilder([]);
		ServiceConfigurator configurator = new();
		builder.Configuration.AddJsonFile(configFile.FullName);
		configurator.ConfigureService(builder.Services, builder.Configuration);

		IHost host = builder.Build();
		return host;
	}
}