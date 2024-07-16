// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP
{
	using System;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using Schleupen.AS4.BusinessAdapter.Configuration;

	public sealed class ReceiveMessageWorker(ILogger<ReceiveMessageWorker> logger, IReceiveMessageAdapterController receiveController, IOptions<ReceiveOptions> receiveOptions)
		: BackgroundService
	{
		private ReceiveOptions options = receiveOptions.Value;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await receiveController.ReceiveAvailableMessagesAsync(stoppingToken);
				}
				catch (CatastrophicException ex)
				{
					logger.LogError(ex, "Catastrophic exception during receive");
					throw;
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Exception during receive");
				}

				await Task.Delay(options.SleepDuration, stoppingToken);
			}
		}
	}
}