// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class CertificateFieldExtensionsTest : IDisposable
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
		public void IsSubjectDistinguishedNameEqualToAs4_WithCertificateWithoutAs4DistinguishedName_ShouldReturnFalse()
		{
			X509Certificate2 testObject = fixture!.CreateTestObject();

			bool result = testObject.IsSubjectDistinguishedNameEqualToAs4();

			Assert.That(result, Is.False);
		}

		[Test]
		public void ResolveFromOrganizationalUnitField_WithCertificateWithOrganizationUnitField_ShouldReturnIdentificationNumber()
		{
			X509Certificate2 testObject = fixture!.CreateTestObject();

			string? result = testObject.ResolveFromOrganizationalUnitField();

			Assert.That(result, Is.EqualTo("9912345000001"));
		}

	}
}
