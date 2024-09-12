namespace Schleupen.AS4.BusinessAdapter.MP.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.MP.Configuration;

public class ReceiveCommand : Command
{
	private readonly ConfigurationFromFileProvider configurationFromFileProvider = new();
	private readonly ServiceConfigurator configurator = new();

	public ReceiveCommand() : base("receive", "receives fp messages from as4 connect")
	{
		var configFileOption = new ConfigFileOption();
		this.AddOption(configFileOption);

		this.SetHandler(Receive, configFileOption);
	}

	private async Task Receive(FileInfo configFile)
	{
		var serviceProvider = CreateServiceProvider(configFile);
		var startUpValidator = serviceProvider.GetRequiredService<IStartupValidator>();
		startUpValidator.Validate();
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