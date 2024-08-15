namespace Schleupen.AS4.BusinessAdapter.FP.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Sending;

public class ReceiveCommand : Command
{
	private readonly ConfigurationFromFileProvider configurationFromFileProvider = new ConfigurationFromFileProvider();
	private readonly ServiceConfigurator configurator = new ServiceConfigurator();

	public ReceiveCommand() : base("receive", "receives fp messages from as4 connect")
	{
		var configFileOption = new ConfigFileOption();
		this.AddOption(configFileOption);

		this.SetHandler(Receive, configFileOption);
	}

	private async Task Receive(FileInfo configFile)
	{
		var serviceProvider = CreateServiceProvider(configFile);
		var sender = serviceProvider.GetRequiredService<IReceiveMessageAdapterController>();

		await sender.ReceiveAvailableMessagesAsync(CancellationToken.None);
	}

	private ServiceProvider CreateServiceProvider(FileInfo configFile)
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddLogging((b) => b.AddConsole());
		configurator.ConfigureReceiving(serviceCollection, configurationFromFileProvider.FromFile(configFile));

		return serviceCollection.BuildServiceProvider();
	}
}