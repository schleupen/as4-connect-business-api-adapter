// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using Microsoft.Extensions.DependencyInjection;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	public class SendMessageAdapterControllerFactory : ISendMessageAdapterControllerFactory
	{
		private readonly IServiceProvider services;

		public SendMessageAdapterControllerFactory(IServiceProvider services)
		{
			this.services = services;
		}

		public ISendMessageAdapterController GetSendController(ControllerType controllerType)
		{
			return controllerType switch
			{
				ControllerType.MP => services.GetRequiredService<SendMessageAdapterController>(),
				ControllerType.FP => services.GetRequiredService<SendMessageAdapterController>(),
				_ => throw new NotImplementedException("Unknown controller type: " + controllerType),
			};
		}
	}
}