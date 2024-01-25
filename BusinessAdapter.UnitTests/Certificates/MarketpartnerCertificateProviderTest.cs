// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class MarketpartnerCertificateProviderTest : IDisposable
	{
		private Fixture? fixture;

		[SetUp]
		public void Setup()
		{
			fixture = new Fixture();
		}

		[TearDown]
		public void Dispose()
		{
			fixture?.Dispose();
			fixture = null;
		}

		[Test]
		public void GetMarketpartnerCertificate_CertificateFound_ShouldReturnCertificate()
		{
			fixture!.PrepareCertificateFound();
			MarketpartnerCertificateProvider testObject = fixture!.CreateTestObject();

			IAs4Certificate certificate = testObject.GetMarketpartnerCertificate("12345");

			Assert.That(certificate, Is.Not.Null);
		}

		[Test]
		public void GetMarketpartnerCertificate_NoAs4CertificateFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareNoAs4CertificateFound();
			MarketpartnerCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetMarketpartnerCertificate("12345"));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}

		[Test]
		public void GetMarketpartnerCertificate_CertificateNotFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareCertificateHasNoMatchingNameFound();
			MarketpartnerCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetMarketpartnerCertificate("12345"));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}

		[Test]
		public void GetMarketpartnerCertificate_MultipleCertificatesFound_ShouldThrowInvalidException()
		{
			fixture!.PrepareMultipleCertificatesFound();
			MarketpartnerCertificateProvider testObject = fixture!.CreateTestObject();

			NoUniqueCertificateException? exception = Assert.Throws<NoUniqueCertificateException>(() => testObject.GetMarketpartnerCertificate("12345"));

			Assert.That(exception!.Message, Contains.Substring("More than one certificate found for the market partner with identification number 12345."));
		}
	}
}
