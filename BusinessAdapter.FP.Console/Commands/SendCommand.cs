namespace Schleupen.AS4.BusinessAdapter.FP.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Sending;

public class SendCommand : Command
{
	private readonly ConfigurationFromFileProvider configurationFromFileProvider = new();
	private readonly ServiceConfigurator configurator = new();

	public SendCommand() : base("send", "sends fp messages to as4 connect")
	{
		var configFileOption = new ConfigFileOption();

		Options.Add(configFileOption);

		SetAction((parseResult, cancellationToken) =>
		{
			var configFile = parseResult.GetValue(configFileOption);

			return Send(configFile!, cancellationToken);
		});
	}

	private Task Send(FileInfo configFile, CancellationToken cancellationToken)
	{
		var serviceProvider = CreateServiceProvider(configFile);

		var startUpValidator = serviceProvider.GetRequiredService<IStartupValidator>();
		startUpValidator.Validate();

		var sender = serviceProvider.GetRequiredService<IFpMessageSender>();
		return sender.SendMessagesAsync(cancellationToken);
	}

	private ServiceProvider CreateServiceProvider(FileInfo configFile)
	{
		var serviceCollection = new ServiceCollection()
			.AddLogging((b) => b.AddConsole());
		configurator.ConfigureSending(serviceCollection, configurationFromFileProvider.FromFile(configFile));

		return serviceCollection.BuildServiceProvider();
	}
}