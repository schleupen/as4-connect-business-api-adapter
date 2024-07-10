// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class SendMessageWorker(
		ILogger<SendMessageWorker> logger,
		ISendMessageAdapterController sendController,
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
					await sendController.SendAvailableMessagesAsync(stoppingToken);
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

				await Task.Delay(TimeSpan.FromSeconds(sendOptions.ScanInterval), stoppingToken);
			}
		}
	}
}
