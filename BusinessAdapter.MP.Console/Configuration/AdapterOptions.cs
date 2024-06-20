// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Configuration
{
	public class AdapterOptions
	{
		public const string Adapter = "Adapter";

		public string SendDirectory { get; set; } = string.Empty;

		public string ReceiveDirectory { get; set; } = string.Empty;

		public string As4ConnectEndpoint { get; set; } = string.Empty;

		public int DeliveryRetryCount { get; set; } = 3;

		public int DeliveryMessageLimitCount { get; set; } = 0;

		public int ReceivingRetryCount { get; set; } = 3;

		public int ReceivingMessageLimitCount { get; set; } = 0;

#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
		public string[]? Marketpartners { get; set; }
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben

		public string CertificateStoreName { get; set; } = "My";

		public string CertificateStoreLocation { get; set; } = "CurrentUser";
	}
}
