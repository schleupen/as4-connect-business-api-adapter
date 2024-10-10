// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using API;
using API.Assemblers;
using Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Configuration;
using Configuration.Validation;
using Gateways;
using Parsing;
using Receiving;
using Sending;
using Sending.Assemblers;

public class ServiceConfigurator
{
	private void ConfigureDefaults(IServiceCollection collection, IConfiguration configuration)
	{
		collection.AddTransient<IJwtBuilder, JwtBuilder>()
			.AddTransient<IClientCertificateProvider, ClientCertificateProvider>()
			.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>()
			.AddTransient<IFileSystemWrapper, FileSystemWrapper>()
			.AddTransient<IFpFileParser, FpFileParser>()
			.AddTransient<IFpParsedFileValidator, FpParsedFileValidator>()
			.AddTransient<IFpOutboxMessageAssembler, FpOutboxMessageAssembler>()
			.AddTransient<IFpFileRepository, FpFileRepository>()
			.AddTransient<IBusinessApiGatewayFactory, BusinessApiGatewayFactory>()
			.AddTransient<IBusinessApiClientFactory, BusinessApiClientFactory>()
			.AddTransient<IHttpClientFactory, HttpClientFactory>()
			.AddTransient<IPartyIdTypeAssembler, PartyIdTypeAssembler>()
			.AddTransient<IFpFileNameExtractor, FpFileNameExtractor>()
			.Configure<EICMapping>(configuration.GetSection(EICMapping.SectionName))
			.AddSingleton<IValidateOptions<Configuration.EICMapping>, EICMappingOptionsValidator>();
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
		collection.AddTransient<IFpMessageReceiver, FpMessageReceiver>();
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