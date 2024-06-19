// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed class PartyInfoTest
	{
		[Test]
		public void Ctor_ShouldSetReceiver()
		{
			ReceivingParty receiver = new ReceivingParty("Identifier", "Type");

			PartyInfo testObject = new PartyInfo(null, receiver);

			Assert.That(testObject.Receiver, Is.EqualTo(receiver));
		}

		[Test]
		public void Ctor_ShouldSetSender()
		{
			SendingParty sender = new SendingParty("Identifier");

			PartyInfo testObject = new PartyInfo(sender, null);

			Assert.That(testObject.Sender, Is.EqualTo(sender));
		}
	}
}
