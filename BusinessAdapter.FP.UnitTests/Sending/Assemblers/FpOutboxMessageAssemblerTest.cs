namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Sending.Assemblers;

using NUnit.Framework;

public partial class FpOutboxMessageAssemblerTest
{
	[Test]
	public void ToFpOutboxMessage_ValidMapping_ShouldAssembleCorrectly()
	{
		var assembler = fixture.CreateTestObjectWithMapping();

		var outboxMessage = assembler.ToFpOutboxMessage(new FpFile(new EIC(TestData.EICCodeSender),
			new EIC(TestData.EICCodeReceiver),
			Array.Empty<byte>(),
			"fileName",
			"path",
			TestData.BDEWProperties));

		Assert.That(outboxMessage.MessageId, Is.Not.Null);
		Assert.That(outboxMessage.MessageId, Is.Not.EqualTo(Guid.Empty));
		Assert.That(outboxMessage.BDEWProperties, Is.EqualTo(TestData.BDEWProperties));
		Assert.That(outboxMessage.Receiver, Is.EqualTo(TestData.Receiver));
		Assert.That(outboxMessage.Sender, Is.EqualTo(TestData.Sender));
		Assert.That(outboxMessage.Payload, Is.EqualTo(outboxMessage.Payload));
		Assert.That(outboxMessage.FilePath, Is.EqualTo(outboxMessage.FilePath));
		Assert.That(outboxMessage.FileName, Is.EqualTo(outboxMessage.FileName));
	}

	[Test]
	public void ToFpOutboxMessage_InvalidSenderMapping_ShouldThrow()
	{
		var assembler = fixture.CreateTestObjectWithMapping();


		Assert.Throws<InvalidOperationException>(() => assembler.ToFpOutboxMessage(new FpFile(new EIC("na"),
			new EIC(TestData.EICCodeReceiver),
			Array.Empty<byte>(),
			"fileName",
			"path",
			TestData.BDEWProperties)));
	}

	[Test]
	public void ToFpOutboxMessage_InvalidReceiverMapping_ShouldThrow()
	{
		var assembler = fixture.CreateTestObjectWithMapping();

		Assert.Throws<InvalidOperationException>(() => assembler.ToFpOutboxMessage(new FpFile(new EIC(TestData.EICCodeSender),
			new EIC("na"),
			Array.Empty<byte>(),
			"fileName",
			"path",
			TestData.BDEWProperties)));
	}
}