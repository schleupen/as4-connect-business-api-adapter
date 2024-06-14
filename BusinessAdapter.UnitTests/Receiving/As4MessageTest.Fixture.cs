// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	internal sealed partial class As4MessageTest
	{
		private sealed class Fixture
		{
			private const string MessageId = "7BBD15F0-BACC-4F0E-981E-4FC767E07570";

			public As4Message CreateTestObject()
			{
				DateTimeOffset createdAt = new DateTimeOffset(new DateTime(2024, 01, 24, 11, 22, 43), TimeSpan.FromHours(1));
				string bdewDocumentDate = "DocumentDate";
				Partyinfo partyinfo = new(new SendingParty("Sender"), new ReceivingParty("Receiver", "BDEW"));

				return new As4Message(createdAt, bdewDocumentDate, MessageId, partyinfo);
			}

			public void ValidateProperties(As4Message testObject)
			{
				Assert.That(testObject.BdewDocumentDate, Is.EqualTo("DocumentDate"));
				Assert.That(testObject.CreatedAt, Is.EqualTo(new DateTimeOffset(new DateTime(2024, 01, 24, 11, 22, 43), TimeSpan.FromHours(1))));
				Assert.That(testObject.MessageId, Is.EqualTo(MessageId));
				Assert.That(testObject.PartyInfo.Receiver!.Id, Is.EqualTo("Receiver"));
				Assert.That(testObject.PartyInfo.Receiver!.Type, Is.EqualTo("BDEW"));
				Assert.That(testObject.PartyInfo.Sender!.Id, Is.EqualTo("Sender"));
			}
		}
	}
}
