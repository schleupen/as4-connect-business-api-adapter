namespace Schleupen.AS4.BusinessAdapter.FP.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class ReceiveCommand : Command
{
	private readonly ConfigurationFromFileProvider configurationFromFileProvider = new();
	private readonly ServiceConfigurator configurator = new();

	public ReceiveCommand() : base("receive", "receives fp messages from as4 connect")
	{
		var configFileOption = new ConfigFileOption();

		Options.Add(configFileOption);

		SetAction((parseResult, cancellationToken) =>
		{
			var configFile = parseResult.GetValue(configFileOption);

			return Receive(configFile!, cancellationToken);
		});
	}

	private Task Receive(FileInfo configFile, CancellationToken cancellationToken)
	{
		var serviceProvider = CreateServiceProvider(configFile);

		var startUpValidator = serviceProvider.GetRequiredService<IStartupValidator>();
		startUpValidator.Validate();

		var sender = serviceProvider.GetRequiredService<IFpMessageReceiver>();
		return sender.ReceiveMessagesAsync(cancellationToken);
	}

	private ServiceProvider CreateServiceProvider(FileInfo configFile)
	{
		var serviceCollection = new ServiceCollection()
			.AddLogging((b) => b.AddConsole());
		configurator.ConfigureReceiving(serviceCollection, configurationFromFileProvider.FromFile(configFile));

		return serviceCollection.BuildServiceProvider();
	}
}