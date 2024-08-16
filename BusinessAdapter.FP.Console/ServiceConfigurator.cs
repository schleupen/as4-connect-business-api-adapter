// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.API.Assemblers;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;
using Schleupen.AS4.BusinessAdapter.FP.Gateways;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;
using Schleupen.AS4.BusinessAdapter.FP.Sending;
using Schleupen.AS4.BusinessAdapter.FP.Sending.Assemblers;

public class ServiceConfigurator
{
	private void ConfigureDefaults(IServiceCollection collection, IConfiguration configuration)
	{
		collection.AddTransient<IJwtBuilder, JwtBuilder>()
			.AddTransient<IClientCertificateProvider, ClientCertificateProvider>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
			.AddTransient<IFpFileParser, FpFileParser>()
			.AddTransient<IFpOutboxMessageAssembler, FpOutboxMessageAssembler>()
			.AddTransient<IFpFileRepository, FpFileRepository>()
			.AddTransient<IBusinessApiGatewayFactory, BusinessApiGatewayFactory>()
			.AddTransient<IBusinessApiClientFactory, BusinessApiClientFactory>()
			.AddTransient<IHttpClientFactory, HttpClientFactory>()
			.AddTransient<IPartyIdTypeAssembler, PartyIdTypeAssembler>()
			.AddTransient<IFpFileNameExtractor, FpFileNameExtractor>()
			.Configure<EICMapping>(configuration.GetSection(EICMapping.SectionName));
	}

	public void ConfigureSending(IServiceCollection collection, IConfiguration configuration)
	{
		ConfigureDefaults(collection, configuration);
		collection.AddSendConfiguration(configuration);
		collection.AddTransient<IFpMessageSender, FpMessageSender>();
	}

	public void ConfigureReceiving(IServiceCollection collection, IConfiguration configuration)
	{
		ConfigureDefaults(collection, configuration);
		collection.AddReceiveConfiguration(configuration);
		collection.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>();
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