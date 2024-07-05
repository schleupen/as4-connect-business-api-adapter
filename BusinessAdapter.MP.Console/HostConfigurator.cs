// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;
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
			.AddTransient<IJwtBuilder, JwtBuilder>()
			.AddTransient<IClientCertificateProvider, ClientCertificateProvider>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
			// MP
			.AddTransient<IBusinessApiGatewayFactory, BusinessApiGatewayFactory>()
			.AddTransient<IBusinessApiClientFactory, BusinessApiClientFactory>()
			.AddHostedService<SendMessageWorker>()
			.AddHostedService<ReceiveMessageWorker>()
			.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>()
			.AddTransient<ISendMessageAdapterController, SendMessageAdapterController>()
			.AddTransient<IEdifactDirectoryResolver, EdifactDirectoryResolver>()
			.AddTransient<IEdifactFileNameExtractor, EdifactFileNameExtractor>()
			.AddTransient<IEdifactFileParser, EdifactFileParser>()
			.AddConfiguration(builder.Configuration);


		IHost host = builder.Build();
		return host;
	}
}