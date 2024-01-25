// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Sending;

	public sealed class SendMessageWorker : BackgroundService
	{
		private readonly ILogger<SendMessageWorker> logger;
		private readonly ISendMessageAdapterController sendController;

		public SendMessageWorker(ILogger<SendMessageWorker> logger, ISendMessageAdapterController sendController)
		{
			this.logger = logger;
			this.sendController = sendController;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					logger.LogDebug("Calling send controller");
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

				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}
		}
	}
}
