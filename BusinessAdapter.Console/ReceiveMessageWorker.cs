// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{

	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.Receiving;
	using System;

	public sealed class ReceiveMessageWorker : BackgroundService
	{
		private readonly ILogger<ReceiveMessageWorker> logger;
		private readonly IReceiveMessageAdapterController receiveController;

		public ReceiveMessageWorker(ILogger<ReceiveMessageWorker> logger, IReceiveMessageAdapterController receiveController)
		{
			this.logger = logger;
			this.receiveController = receiveController;
		}

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

				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}
		}
	}
}