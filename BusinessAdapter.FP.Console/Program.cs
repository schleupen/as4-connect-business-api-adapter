// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP
{
	using System.CommandLine;
	using System.Threading.Tasks;
	using Schleupen.AS4.BusinessAdapter.FP.Commands;

	public partial class Program
	{
		public static async Task Main(string[] args)
		{
			RootCommand command =
			[
				new SendCommand(),
				new ReceiveCommand(),
				new ServiceCommand()
			];

			await command.InvokeAsync(args);
		}
	}
}