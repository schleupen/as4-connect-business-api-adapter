// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP
{
	using System.CommandLine;
	using Schleupen.AS4.BusinessAdapter.MP.Commands;

	public static class Program
	{
		public static async Task<int> Main(string[] args)
		{
			RootCommand command =
			[
				new SendCommand(),
				new ReceiveCommand(),
				new ServiceCommand()
			];

			using (var cancellationTokenSource = new CancellationTokenSource())
			{
				return await command.Parse(args).InvokeAsync(cancellationToken: cancellationTokenSource.Token).ConfigureAwait(ConfigureAwaitOptions.None);
			}
		}
	}
}