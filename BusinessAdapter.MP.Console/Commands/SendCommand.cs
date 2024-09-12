namespace Schleupen.AS4.BusinessAdapter.MP.Commands;

using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.MP.Sending;

public class SendCommand : Command
{
	public SendCommand() : base("send", "sends mp messages to as4 connect")
	{
		var configFileOption = new ConfigFileOption();
		this.AddOption(configFileOption);

		this.SetHandler(Send, configFileOption);
	}

	private async Task Send(FileInfo configFile)
	{
		var serviceProvider = CreateServiceProvider(configFile);

		var startUpValidator = serviceProvider.GetRequiredService<IStartupValidator>();
		startUpValidator.Validate();

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
		return serviceCollection.BuildServiceProvider();
	}
}