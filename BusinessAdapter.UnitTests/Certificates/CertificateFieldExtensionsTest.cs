// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System;
	using System.Security.Cryptography.X509Certificates;
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class CertificateFieldExtensionsTest : IDisposable
	{
		private Fixture fixture;

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
		public void IsSubjectDistinguishedNameEqualToAs4_ApiCertificate_ShouldReturnFalse()
		{
			X509Certificate2 testObject = fixture.CreateApiCertificate();

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.False);
		}

		[Test]
		public void IsSubjectDistinguishedNameEqualToAs4_UnspecificCertificate_ShouldReturnFalse()
		{
			X509Certificate2 testObject = fixture.CreateUnspecificCertificate();

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.False);
		}

		[Test]
		public void IsSubjectDistinguishedNameEqualToAs4_AS4Certificate_ShouldReturnTrue()
		{
			X509Certificate2 testObject = fixture.CreateAs4Certificate();

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.True);
		}

		[Test]
		public void ResolveMarketpartnerIdentificationNumber_UnspecificCertificate_ShouldReturnIdentificationNumber()
		{
			X509Certificate2 testObject = fixture.CreateUnspecificCertificate();

			var result = testObject.ResolveMarketpartnerIdentificationNumber();

			Assert.That(result, Is.EqualTo("9912345000001"));
		}

		[Test]
		public void ResolveMarketpartnerIdentificationNumber_AS4Certificate_ShouldReturnIdentificationNumber()
		{
			X509Certificate2 testObject = fixture.CreateAs4Certificate();

			var result = testObject.ResolveMarketpartnerIdentificationNumber();

			Assert.That(result, Is.EqualTo("9999999999999"));
		}

		[Test]
		public void ResolveMarketpartnerIdentificationNumber_ApiCertificate_ShouldReturnIdentificationNumber()
		{
			X509Certificate2 testObject = fixture.CreateApiCertificate();

			var result = testObject.ResolveMarketpartnerIdentificationNumber();

			Assert.That(result, Is.EqualTo(null));
		}
	}
}
