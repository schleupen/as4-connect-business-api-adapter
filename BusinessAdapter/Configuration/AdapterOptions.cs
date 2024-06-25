// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Configuration
{
	using System.Security.Cryptography.X509Certificates;

	public record AdapterOptions
	{
		public const string SectionName = "Adapter";
		public const string ReceiveSectionName = SectionName + ":" + nameof(Receive);
		public const string SendSectionName = SectionName + ":" + nameof(Send);

		public SendOptions? Send { get; set; }

		public ReceiveOptions? Receive { get; set; }

		public StoreName CertificateStoreName { get; set; } = StoreName.My;

		public StoreLocation CertificateStoreLocation { get; set; } = StoreLocation.CurrentUser;

		public string As4ConnectEndpoint { get; set; } = string.Empty;

#pragma warning disable CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
		public string[]? Marketpartners { get; set; }
#pragma warning restore CA1819 // Eigenschaften dürfen keine Arrays zurückgeben
	}
}