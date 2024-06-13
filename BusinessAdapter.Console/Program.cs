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
			HostConfigurator configurator = new HostConfigurator();
			IHost host = configurator.ConfigureHost(args);

			await host.RunAsync();
		}
	}
}