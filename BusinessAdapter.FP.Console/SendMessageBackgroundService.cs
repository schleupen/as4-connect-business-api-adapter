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

	public sealed class SendMessageBackgroundService(
		ILogger<SendMessageBackgroundService> logger,
		IFpMessageSender sender,
		IOptions<SendOptions> sendOptions)
		: BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await sender.SendMessagesAsync(stoppingToken);
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

				logger.LogInformation("Next sending iteration is scheduled in '{SleepDuration}' at '{ScheduleTime}'", sendOptions.Value.SleepDuration, DateTime.Now + sendOptions.Value.SleepDuration);
				await Task.Delay(sendOptions.Value.SleepDuration, stoppingToken);
			}
		}
	}
}