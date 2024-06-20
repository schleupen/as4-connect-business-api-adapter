// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	public class AdapterConfiguration(int deliveryRetryCount, int deliveryMessageLimitCount, int receivingRetryCount, int receivingMessageLimitCount)
	{
		public int DeliveryRetryCount { get; } = deliveryRetryCount;

		public int ReceivingRetryCount { get; } = receivingRetryCount;

		public int ReceivingMessageLimitCount { get; } = receivingMessageLimitCount;

		public int DeliveryMessageLimitCount
		{
			get
			{
				return deliveryMessageLimitCount <= 0 ? int.MaxValue : deliveryMessageLimitCount;
			}
		}
	}
}
