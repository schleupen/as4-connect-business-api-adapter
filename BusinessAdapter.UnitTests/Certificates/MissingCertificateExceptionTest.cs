// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed class MissingCertificateExceptionTest
	{
		[Test]
		public void Ctor_ShouldSetIdentificationNumber()
		{
			MissingCertificateException testObject = new MissingCertificateException("Identification Number");

			Assert.That(testObject.MarketpartnerIdentificationNumber, Is.EqualTo("Identification Number"));
		}
	}
}
