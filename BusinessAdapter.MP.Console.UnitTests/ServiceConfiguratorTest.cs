// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;
using Schleupen.AS4.BusinessAdapter.MP.Sending;

[TestFixture]
internal sealed partial class ServiceConfiguratorTest
{
	private readonly ServiceConfiguratorTest.Fixture fixture = new();
	private IConfiguration config = new ConfigurationBuilder().Build();
	private ServiceCollection serviceCollection = new();

	[SetUp]
	public void Setup()
	{
		config = new ConfigurationBuilder().AddJsonFile("appsettings.unittests.json").Build();
		serviceCollection = new ServiceCollection();
	}

	[Test]
	public void ConfigureSending_ShouldRegisterRootComponent()
	{
		ServiceConfigurator configurator = fixture.CreateTestObject();

		configurator.ConfigureSending(serviceCollection, config);
		serviceCollection.AddLogging((b) => b.AddConsole());

		var sender = serviceCollection.BuildServiceProvider().GetRequiredService<ISendMessageAdapterController>();
		Assert.That(sender, Is.Not.Null);
	}

	[Test]
	public void ConfigureReceiving_ShouldRegisterRootComponent()
	{
		ServiceConfigurator configurator = fixture.CreateTestObject();

		configurator.ConfigureReceiving(serviceCollection, config);
		serviceCollection.AddLogging((b) => b.AddConsole());

		var sender = serviceCollection.BuildServiceProvider().GetRequiredService<IReceiveMessageAdapterController>();
		Assert.That(sender, Is.Not.Null);
	}

	[Test]
	public void ConfigureService_ShouldRegisterBackgroundServices()
	{
		ServiceConfigurator configurator = fixture.CreateTestObject();

		configurator.ConfigureService(serviceCollection, config);
		serviceCollection.AddLogging((b) => b.AddConsole());

		var serviceProvider = serviceCollection.BuildServiceProvider();
		var hostedServices = serviceProvider.GetServices<IHostedService>();

		Assert.That(hostedServices, Has.Exactly(1).TypeOf<SendMessageBackgroundService>());
		Assert.That(hostedServices, Has.Exactly(1).TypeOf<ReceiveMessageBackgroundService>());
	}
}