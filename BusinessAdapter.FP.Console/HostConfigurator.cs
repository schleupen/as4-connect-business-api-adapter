﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
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
			.Configure<EICMapping>(builder.Configuration.GetSection(EICMapping.SectionName))
			.AddConfiguration(builder.Configuration);

		IHost host = builder.Build();
		return host;
	}
}