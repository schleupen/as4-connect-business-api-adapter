// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;
	using Schleupen.AS4.BusinessAdapter.FP.Sending;

	public sealed class SendMessageWorker(
		ILogger<SendMessageWorker> logger,
		IFpMessageSender sender,
		IOptions<SendOptions> sendOptions)
		: BackgroundService
	{
		private readonly SendOptions sendOptions = sendOptions.Value;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await sender.SendAvailableMessagesAsync(stoppingToken);
				}
				catch (CatastrophicException ex)
				{
					logger.LogError(ex, "Catastrophic exception while sending");
					throw;
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error while sending messages");
				}

				await Task.Delay(sendOptions.ScanInterval, stoppingToken);
			}
		}
	}
}
