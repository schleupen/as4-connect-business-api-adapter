// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System;
	using System.Security.Cryptography.X509Certificates;
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class CertificateFieldExtensionsTest : IDisposable
	{
		private CertificateFieldExtensionsTest.Fixture fixture;

		[SetUp]
		public void Setup()
		{
			fixture = new CertificateFieldExtensionsTest.Fixture();
		}

		[TearDown]
		public void Dispose()
		{
			fixture?.Dispose();
			fixture = null;
		}

		[Test]
		public void IsSubjectDistinguishedNameEqualToAs4_WithCertificateWithoutAs4DistinguishedName_ShouldReturnFalse()
		{
			X509Certificate2 testObject = fixture.ReadCertificateFromResource("client_unspecific_certificate.pfx");

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.False);
		}

		[Test]
		public void IsSubjectDistinguishedNameEqualToAs4_WithAS4Certificate_ShouldReturnTrue()
		{
			X509Certificate2 testObject = fixture.ReadCertificateFromResource("client_as4_certificate.pfx");

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.True);
		}

		[Test]
		public void IsSubjectDistinguishedNameEqualToAs4_WithApiCertificate_ShouldReturnTrue()
		{
			X509Certificate2 testObject = fixture.ReadCertificateFromResource("client_api_certificate.pfx");

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.False);
		}

		[Test]
		public void ResolveMarketpartnerIdentificationNumber_WithCertificateWithOrganizationUnitField_ShouldReturnIdentificationNumber()
		{
			X509Certificate2 testObject = fixture.ReadCertificateFromResource("client_as4_certificate.pfx");

			var result = testObject.ResolveMarketpartnerIdentificationNumber();

			Assert.That(result, Is.EqualTo("9999999999999"));
		}
	}
}
