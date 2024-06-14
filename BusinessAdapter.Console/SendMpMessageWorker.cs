﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System;
	using System.Threading.Tasks;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;

	public sealed class SendMpMessageWorker : BackgroundService
	{
		private readonly ILogger<SendMpMessageWorker> logger;
		private readonly ISendMessageAdapterControllerFactory sendControllerFactory;

		public SendMpMessageWorker(ILogger<SendMpMessageWorker> logger, ISendMessageAdapterControllerFactory sendControllerFactory)
		{
			this.logger = logger;
			this.sendControllerFactory = sendControllerFactory;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			ISendMessageAdapterController sendController = sendControllerFactory.GetSendController(ControllerType.MP);

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
