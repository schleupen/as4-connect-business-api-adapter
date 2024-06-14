// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Sending;

	public sealed class SendFpMessageWorker : BackgroundService
	{
		private readonly ILogger<SendFpMessageWorker> logger;
		private readonly ISendMessageAdapterControllerFactory sendControllerFactory;

		public SendFpMessageWorker(ILogger<SendFpMessageWorker> logger, ISendMessageAdapterControllerFactory sendControllerFactory)
		{
			this.logger = logger;
			this.sendControllerFactory = sendControllerFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

			ISendMessageAdapterController sendController = sendControllerFactory.GetSendController(ControllerType.FP);

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

				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}
		}
	}
}
