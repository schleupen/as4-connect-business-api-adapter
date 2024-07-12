// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.API;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	internal sealed partial class MpOutboxMessageTest
	{
		private sealed class Fixture
		{
			private const string SenderMessageId = "960A5057-529F-4A9F-B5FA-73551867BEAB";

			public MpOutboxMessage CreateTestObject()
			{
				ReceivingParty receiver = new ReceivingParty("Reciever", "BDEW");
				string senderMessageId = SenderMessageId;
				string bdewDocumentNumber = "DocumentNumber";
				string bdewDocumentType = "DocumentType";
				string fileName = "test.edi";
				DateTimeOffset documentDate = new DateTimeOffset(new DateTime(2024, 01, 24, 11, 10, 43), TimeSpan.FromHours(1));

				return new MpOutboxMessage(receiver, senderMessageId, bdewDocumentNumber, bdewDocumentType, Array.Empty<byte>(), fileName, documentDate);
			}

			public void ValidateProperties(MpOutboxMessage testObject)
			{
				Assert.That(testObject.BdewDocumentNumber, Is.EqualTo("DocumentNumber"));
				Assert.That(testObject.BdewDocumentType, Is.EqualTo("DocumentType"));
				Assert.That(testObject.DocumentDate, Is.EqualTo(new DateTimeOffset(new DateTime(2024, 01, 24, 11, 10, 43), TimeSpan.FromHours(1))));
				Assert.That(testObject.FileName, Is.EqualTo("test.edi"));
				Assert.That(testObject.Payload, Is.EqualTo(Array.Empty<byte>()));
				Assert.That(testObject.Receiver.Id, Is.EqualTo("Reciever"));
				Assert.That(testObject.Receiver.Type, Is.EqualTo("BDEW"));
				Assert.That(testObject.SenderMessageId, Is.EqualTo(SenderMessageId));
			}
		}
	}
}
