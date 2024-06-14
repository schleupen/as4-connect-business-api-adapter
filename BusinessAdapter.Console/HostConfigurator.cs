// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.Receiving;
using Schleupen.AS4.BusinessAdapter.Sending;

public class HostConfigurator
{
	public IHost ConfigureHost(string[] args)
	{
		HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
		builder.Services
			.AddHostedService<SendMpMessageWorker>()
			.AddHostedService<ReceiveMessageWorker>()
			.AddTransient<ISendMessageAdapterControllerFactory, SendMessageAdapterControllerFactory>()
			.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>()
			.AddTransient<ISendMessageAdapterController, SendMessageAdapterController>()
			.AddTransient<IAs4BusinessApiClientFactory, As4BusinessApiClientFactory>()
			.AddTransient<IJwtHelper, JwtHelper>()
			.AddTransient<IMarketpartnerCertificateProvider, MarketpartnerCertificateProvider>()
			.AddSingleton<IConfigurationAccess, ConfigurationAccess>()
			.AddTransient<IEdifactDirectoryResolver, EdifactDirectoryResolver>()
			.AddTransient<IFileNameExtractor, FileNameExtractor>()
			.AddTransient<IClientWrapperFactory, ClientWrapperFactory>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IEdifactFileParser, EdifactFileParser>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
			.Configure<AdapterOptions>(builder.Configuration.GetSection(AdapterOptions.Adapter));

		IHost host = builder.Build();
		return host;
	}
}