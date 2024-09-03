// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed class SendingPartyTest
	{
		[Test]
		public void Ctor_ShouldSetIdentificationNumber()
		{
			SendingParty testObject = new SendingParty("Identifier", "type");

			Assert.That(testObject.Id, Is.EqualTo("Identifier"));
			Assert.That(testObject.Type, Is.EqualTo("type"));
		}
	}
}
