// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Console;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Sending;


public class HostConfigurator
{
	public IHost ConfigureHost(string[] args)
	{
		HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
		builder.Services
			// Common
			.AddTransient<IJwtHelper, JwtHelper>()
			.AddTransient<IMarketpartnerCertificateProvider, MarketpartnerCertificateProvider>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
			// FP
			.AddHostedService<SendMessageWorker>()
			.AddHostedService<ReceiveMessageWorker>()
			.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>()
			.AddTransient<ISendMessageAdapterController, SendMessageAdapterController>()
			// Config
			.Configure<Configuration.AdapterOptions>(builder.Configuration.GetSection(Configuration.AdapterOptions.SectionName))
			.Configure<SendOptions>(builder.Configuration.GetSection(Configuration.AdapterOptions.SendSectionName))
			.Configure<ReceiveOptions>(builder.Configuration.GetSection(Configuration.AdapterOptions.ReceiveSectionName))
			.AddSingleton<IValidateOptions<Configuration.AdapterOptions>, AdapterOptionsValidator>()
			.AddOptionsWithValidateOnStart<Configuration.AdapterOptions>();


		IHost host = builder.Build();
		return host;
	}
}