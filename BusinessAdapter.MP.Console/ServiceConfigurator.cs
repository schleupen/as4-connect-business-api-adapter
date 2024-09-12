// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.API.Assemblers;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.MP.API;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;
using Schleupen.AS4.BusinessAdapter.MP.Sending;

public class ServiceConfigurator
{
	private void ConfigureDefaults(IServiceCollection collection, IConfiguration configuration)
	{
		collection.AddTransient<IJwtBuilder, JwtBuilder>()
			.AddTransient<IClientCertificateProvider, ClientCertificateProvider>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
			.AddTransient<IJwtBuilder, JwtBuilder>()
			.AddTransient<IBusinessApiGatewayFactory, BusinessApiGatewayFactory>()
			.AddTransient<IBusinessApiClientFactory, BusinessApiClientFactory>()
			.AddTransient<IHttpClientFactory, HttpClientFactory>()
			.AddTransient<IPartyIdTypeAssembler, PartyIdTypeAssembler>()
			.AddTransient<IEdifactDirectoryResolver, EdifactDirectoryResolver>()
			.AddTransient<IEdifactFileNameExtractor, EdifactFileNameExtractor>()
			.AddTransient<IEdifactFileParser, EdifactFileParser>();
	}

	public void ConfigureSending(IServiceCollection collection, IConfiguration configuration)
	{
		ConfigureDefaults(collection, configuration);
		collection.AddSendConfiguration(configuration);
		collection.AddTransient<IMpMessageSender, MpMessageSender>();
	}

	public void ConfigureReceiving(IServiceCollection collection, IConfiguration configuration)
	{
		ConfigureDefaults(collection, configuration);
		collection.AddReceiveConfiguration(configuration);
		collection.AddTransient<IMpMessageReceiver, MpMessageReceiver>();
	}

	public void ConfigureService(IServiceCollection collection, IConfiguration configuration)
	{
		ConfigureDefaults(collection, configuration);
		ConfigureSending(collection, configuration);
		ConfigureReceiving(collection, configuration);

		collection.AddHostedService<SendMessageBackgroundService>();
		collection.AddHostedService<ReceiveMessageBackgroundService>();
	}
}