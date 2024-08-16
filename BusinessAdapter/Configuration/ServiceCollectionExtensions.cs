namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;

public static class ServiceCollectionExtensions
{
	private static IServiceCollection AddAdapterConfiguration(this IServiceCollection services,
		Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
		services.Configure<Configuration.AdapterOptions>(configuration.GetSection(Configuration.AdapterOptions.SectionName))
			.AddSingleton<IValidateOptions<Configuration.AdapterOptions>, AdapterOptionsValidator>()
			.AddOptionsWithValidateOnStart<Configuration.AdapterOptions>();
		return services;
	}


	public static IServiceCollection AddSendConfiguration(this IServiceCollection services,
		Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
		services.AddAdapterConfiguration(configuration);

		services
			.Configure<SendOptions>(configuration.GetSection(Configuration.SendOptions.SendSectionName))
			.AddSingleton<IValidateOptions<Configuration.SendOptions>, SendOptionsValidator>()
			.AddOptionsWithValidateOnStart<Configuration.SendOptions>();
		return services;
	}

	public static IServiceCollection AddReceiveConfiguration(this IServiceCollection services,
		Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
		services.AddAdapterConfiguration(configuration);
		services.Configure<ReceiveOptions>(configuration.GetSection(Configuration.ReceiveOptions.ReceiveSectionName));
		services.AddSingleton<IValidateOptions<Configuration.ReceiveOptions>, ReceiveOptionsValidator>();
		services.AddOptionsWithValidateOnStart<Configuration.ReceiveOptions>();
		return services;
	}

	public static IServiceCollection AddSendAndReceiveConfiguration(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
		services.AddAdapterConfiguration(configuration);
		services.AddSendConfiguration(configuration);
		services.AddReceiveConfiguration(configuration);

		return services;
	}
}