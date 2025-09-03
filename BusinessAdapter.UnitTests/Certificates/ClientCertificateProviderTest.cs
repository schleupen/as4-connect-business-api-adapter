// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class ClientCertificateProviderTest : IDisposable
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
		public void GetCertificate_ValidCertificateFound_ShouldReturnCertificate()
		{
			fixture!.PrepareCertificateFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			IClientCertificate certificate = testObject.GetCertificate(fixture.MarktPartnerId);

			Assert.That(certificate, Is.Not.Null);
		}

		[Test]
		public void GetCertificate_CertificateForMPNotFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareCertificateHasNoMatchingNameFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetCertificate(fixture.MarktPartnerId));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}

		[Test]
		public void GetCertificate_CertificateWithOldTimestampNotFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareCertificateWithOldTimestampNotFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetCertificate(fixture.MarktPartnerId));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}

		[Test]
		public void GetCertificate_CertificateWithNewTimestampNotFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareCertificateWithNewTimestampNotFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetCertificate(fixture.MarktPartnerId));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}

		[Test]
		public void GetCertificate_NoCertificateFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareNoCertificateFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetCertificate(fixture.MarktPartnerId));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}

		[Test]
		public void GetCertificate_MultipleValidCertificatesFound_ShouldReturnNewest()
		{
			fixture!.PrepareMultipleValidCertificatesFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			IClientCertificate certificate = testObject.GetCertificate(fixture.MarktPartnerId);

			Assert.That(certificate, Is.Not.Null);
			fixture.IsCertificateTwo(certificate);
		}

		[Test]
		public void GetCertificate_MultipleCertificatesWithOneValidAndOneInvalidWithWrongMPFound_ShouldReturnValidCertificate()
		{
			fixture!.PrepareMultipleCertificatesWithOneValidAndOneInvalidWithWrongMPFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			IClientCertificate certificate = testObject.GetCertificate(fixture.MarktPartnerId);

			Assert.That(certificate, Is.Not.Null);
			fixture.IsCertificateOne(certificate);
		}

		[Test]
		public void GetCertificate_MultipleCertificatesWithOneValidAndOneInvalidWithOldTimestampFound_ShouldReturnValidCertificate()
		{
			fixture!.PrepareMultipleCertificatesWithOneValidAndOneInvalidWithOldTimestampFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			IClientCertificate certificate = testObject.GetCertificate(fixture.MarktPartnerId);

			Assert.That(certificate, Is.Not.Null);
			fixture.IsCertificateOne(certificate);
		}

		[Test]
		public void GetCertificate_MultipleCertificatesWithOneValidAndOneInvalidWithNewTimestampFound_ShouldReturnValidCertificate()
		{
			fixture!.PrepareMultipleCertificatesWithOneValidAndOneInvalidWithNewTimestampFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			IClientCertificate certificate = testObject.GetCertificate(fixture.MarktPartnerId);

			Assert.That(certificate, Is.Not.Null);
			fixture.IsCertificateOne(certificate);
		}

		[Test]
		public void GetCertificate_MultipleCertificatesWithNoValidCertificatesFound_ShouldThrowMissingCertificateException()
		{
			fixture!.PrepareMultipleCertificatesWithNoValidCertificatesFound();
			ClientCertificateProvider testObject = fixture!.CreateTestObject();

			MissingCertificateException? exception = Assert.Throws<MissingCertificateException>(() => testObject.GetCertificate(fixture.MarktPartnerId));

			Assert.That(exception!.Message, Contains.Substring("No certificate found for the market partner with identification number 12345."));
		}
	}
}
