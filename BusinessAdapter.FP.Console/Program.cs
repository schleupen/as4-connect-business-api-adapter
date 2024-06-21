// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Console
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;

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