namespace Schleupen.AS4.BusinessAdapter.FP;

using NUnit.Framework;

public sealed partial class SendAndReceiveTests : IDisposable
{
	[Test]
	public async Task Send_ValidFilesInSendDirectory_ShouldSend()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithCertificate);
		fixture.AddFileToSendDirectory();

		var sendStatus = await this.fixture.Send();

		Assert.That(sendStatus.FailedMessages.Count, Is.Zero);
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(1));

		fixture.VerifySendDirectoryIsEmpty();
	}

	[Test]
	public async Task Receive_InboxHasMessages_ShouldSaveFilesInReceiveDirectory()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithCertificate);

		var receiveStatus = await this.fixture.Receive();

		Assert.That(receiveStatus.FailedMessages.Count, Is.Zero);
		Assert.That(receiveStatus.SuccessfulMessages.Count, Is.EqualTo(4));

		fixture.VerifyReceiveDirectoryIsNotEmpty();
	}

	[Test]
	public void Receive_MissingCertificate_ShouldThrowException()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithoutCertificate);

		var exception = Assert.ThrowsAsync<AggregateException>(() => this.fixture.Receive());

		Assert.That(exception, Is.Not.Null);
		Assert.That(exception!.InnerExceptions, Is.Not.Empty);
		Assert.That(exception!.InnerExceptions[0].Message, Is.EqualTo($"No certificate found for the market partner with identification number {TestData.MarketpartnerIdWithoutCertificate}."));

		fixture.VerifyReceiveDirectoryIsEmpty();
	}

	[Test]
	public void Receive_MissingMapping_ShouldThrowException()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithoutMapping);

		var exception = Assert.ThrowsAsync<AggregateException>(() => this.fixture.Receive());

		Assert.That(exception, Is.Not.Null);
		Assert.That(exception!.InnerExceptions, Is.Not.Empty);
		Assert.That(exception!.InnerExceptions[0].Message, Is.EqualTo($"Receiving party {TestData.MarketpartnerIdWithoutMapping} mapping not configured"));

		fixture.VerifyReceiveDirectoryIsEmpty();
	}
}