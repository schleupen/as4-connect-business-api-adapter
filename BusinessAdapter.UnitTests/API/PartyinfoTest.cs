// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using NUnit.Framework;

	[TestFixture]
	internal sealed class PartyinfoTest
	{
		[Test]
		public void Ctor_ShouldSetReceiver()
		{
			ReceivingParty receiver = new ReceivingParty("Identifier", "Type");

			Partyinfo testObject = new Partyinfo(null, receiver);

			Assert.That(testObject.Receiver, Is.EqualTo(receiver));
		}

		[Test]
		public void Ctor_ShouldSetSender()
		{
			SendingParty sender = new SendingParty("Identifier");

			Partyinfo testObject = new Partyinfo(sender, null);

			Assert.That(testObject.Sender, Is.EqualTo(sender));
		}
	}
}
