// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.MP.API;
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
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
				// MP
			.AddTransient<IAs4BusinessApiClientFactory, As4BusinessApiClientFactory>()
			.AddTransient<IClientWrapperFactory, ClientWrapperFactory>()
			.AddHostedService<SendMessageWorker>()
			.AddHostedService<ReceiveMessageWorker>()
			.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>()
			.AddTransient<ISendMessageAdapterController, SendMessageAdapterController>()
			.AddTransient<IEdifactDirectoryResolver, EdifactDirectoryResolver>()
			.AddTransient<IEdifactFileNameExtractor, EdifactFileNameExtractor>()
			.AddTransient<IEdifactFileParser, EdifactFileParser>()

			.Configure<AdapterOptions>(builder.Configuration.GetSection(AdapterOptions.SectionName));

		IHost host = builder.Build();
		return host;
	}
}