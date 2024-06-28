// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP
{
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;

	public class Program
	{
		public static async Task Main(string[] args)
		{
			HostConfigurator configurator = new HostConfigurator();
			IHost host = configurator.ConfigureHost(args);

			await host.RunAsync();
		}
	}
}