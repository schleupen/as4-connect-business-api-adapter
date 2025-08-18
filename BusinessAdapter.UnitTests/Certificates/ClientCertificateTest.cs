// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.IdentityModel.Tokens;
	using NUnit.Framework;

	[TestFixture]
	internal sealed partial class ClientCertificateTest : IDisposable
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
		public void IsCertificateFor_CertificateHasOtherIdentificationNumber_ShouldReturnFalse()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			bool result = testObject.IsCertificateFor("Test123");

			Assert.That(result, Is.False);
		}

		[Test]
		public void IsCertificateFor_CertificateHasMatchingIdentificationNumber_ShouldReturnTrue()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			bool result = testObject.IsCertificateFor("9912345000001");

			Assert.That(result, Is.True);
		}

		[Test]
		public void AsX509Certificate_ShouldReturnX509Certificate()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			X509Certificate result = testObject.AsX509Certificate();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Subject, Is.EqualTo("OU=9912345000001@BDEW"));
		}

		[Test]
		public void GetPrivateSecurityKey_ShouldReturnPrivateKey()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			SecurityKey result = testObject.GetPrivateSecurityKey();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void GetRawCertData_ShouldReturnRawCertificateData()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			byte[] result = testObject.GetRawCertData();

			Assert.That(result, Is.Not.Null);
		}

		[Test]
		public void ValidFrom_ShouldReturnCorrectTimestamp()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			DateTime result = testObject.ValidFrom;
			DateTime expected = new DateTime(2023, 7, 27, 16, 58, 04);

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void ValidUntil_ShouldReturnCorrectTimestamp()
		{
			ClientCertificate testObject = fixture!.CreateTestObject();

			var result = testObject.ValidUntil;
			DateTime expected = new DateTime(2031, 12, 17, 15, 58, 04);

			Assert.That(result, Is.EqualTo(expected));
		}
	}
}
