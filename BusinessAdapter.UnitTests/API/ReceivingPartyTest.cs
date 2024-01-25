// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed class ReceivingPartyTest
	{
		[Test]
		public void Ctor_ShouldSetIdentificationNumberAndType()
		{
			ReceivingParty testObject = new ReceivingParty("Identification Number", "Type");

			Assert.That(testObject.Id, Is.EqualTo("Identification Number"));
			Assert.That(testObject.Type, Is.EqualTo("Type"));
		}
	}
}
