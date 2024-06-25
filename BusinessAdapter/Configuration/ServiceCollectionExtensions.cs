namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.Configuration.Validation;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddConfiguration(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
	{
		services.Configure<Configuration.AdapterOptions>(configuration.GetSection(Configuration.AdapterOptions.SectionName))
			.Configure<SendOptions>(configuration.GetSection(Configuration.AdapterOptions.SendSectionName))
			.Configure<ReceiveOptions>(configuration.GetSection(Configuration.AdapterOptions.ReceiveSectionName))
			.AddSingleton<IValidateOptions<Configuration.AdapterOptions>, AdapterOptionsValidator>()
			.AddOptionsWithValidateOnStart<Configuration.AdapterOptions>();

		return services;
	}
}