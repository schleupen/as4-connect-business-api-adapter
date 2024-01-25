// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Schleupen.AS4.BusinessAdapter.Receiving;
	using Schleupen.AS4.BusinessAdapter.Sending;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.Certificates;

	public static class Program
	{
		public static async Task Main(string[] args)
		{
			HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
			builder.Services.AddHostedService<SendMessageWorker>();
			builder.Services.AddHostedService<ReceiveMessageWorker>();

			builder.Services.AddTransient<IReceiveMessageAdapterController, ReceiveMessageAdapterController>();
			builder.Services.AddTransient<ISendMessageAdapterController, SendMessageAdapterController>();

			builder.Services.AddTransient<IAs4BusinessApiClientFactory, As4BusinessApiClientFactory>();
			builder.Services.AddTransient<IJwtHelper, JwtHelper>();
			builder.Services.AddTransient<IMarketpartnerCertificateProvider, MarketpartnerCertificateProvider>();
			builder.Services.AddSingleton<IConfigurationAccess, ConfigurationAccess>();
			builder.Services.AddTransient<IEdifactDirectoryResolver, EdifactDirectoryResolver>();
			builder.Services.AddTransient<IClientWrapperFactory, ClientWrapperFactory>();
			builder.Services.AddTransient<ICertificateStoreFactory, CertificateStoreFactory>();

			builder.Services.Configure<AdapterOptions>(
				builder.Configuration.GetSection(AdapterOptions.Adapter));

			IHost host = builder.Build();
			await host.RunAsync();
		}
	}
}