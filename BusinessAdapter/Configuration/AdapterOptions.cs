// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	public class AdapterConfiguration
	{
		private int deliveryMessageLimitCount;

		public AdapterConfiguration(int deliveryRetryCount, int deliveryMessageLimitCount, int receivingRetryCount, int receivingMessageLimitCount)
		{
			DeliveryRetryCount = deliveryRetryCount;
			this.deliveryMessageLimitCount = deliveryMessageLimitCount;
			ReceivingRetryCount = receivingRetryCount;
			ReceivingMessageLimitCount = receivingMessageLimitCount;
		}

		public int DeliveryRetryCount { get; }

		public int ReceivingRetryCount { get; }

		public int ReceivingMessageLimitCount { get; }

		public int DeliveryMessageLimitCount
		{
			get
			{
				return deliveryMessageLimitCount <= 0 ? int.MaxValue : deliveryMessageLimitCount;
			}
		}
	}
}
