// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.MP;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;
using Schleupen.AS4.BusinessAdapter.MP.Sending;

public class HostConfigurator
{
	public IHost ConfigureHost(string[] args)
	{
		HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
		builder.Services
				// Common
			.AddTransient<IJwtHelper, JwtHelper>()
			.AddTransient<IMarketpartnerCertificateProvider, MarketpartnerCertificateProvider>()
			.AddSingleton<IConfigurationAccess, ConfigurationAccess>()
			.AddTransient<IAs4BusinessApiClientFactory, As4BusinessApiClientFactory>()
			.AddTransient<ISendMessageAdapterControllerFactory, SendMessageAdapterControllerFactory>()
			.AddTransient<IClientWrapperFactory, ClientWrapperFactory>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
				// MP
			.AddHostedService<SendMpMessageWorker>()
			.AddHostedService<ReceiveMessageWorker>()
			.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>()
			.AddTransient<ISendMessageAdapterController, SendMessageAdapterController>()
			.AddTransient<IEdifactDirectoryResolver, EdifactDirectoryResolver>()
			.AddTransient<IEdifactFileNameExtractor, EdifactFileNameExtractor>()
			.AddTransient<IEdifactFileParser, EdifactFileParser>()
				// FP
				// ...

			.Configure<AdapterOptions>(builder.Configuration.GetSection(AdapterOptions.Adapter));

		IHost host = builder.Build();
		return host;
	}
}