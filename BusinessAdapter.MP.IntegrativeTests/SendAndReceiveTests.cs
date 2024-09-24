namespace Schleupen.AS4.BusinessAdapter.MP;

using NUnit.Framework;

public sealed partial class SendAndReceiveTests : IDisposable
{
	[Test]
	public async Task Send_ValidFilesInSendDirectory_ShouldSend()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithCertificate);
		fixture.AddFileFromValidMarketpartnerToSendDirectory();

		await this.fixture.Send();

		fixture.VerifySendDirectoryIsEmpty();
	}

	[Test]
	public async Task Receive_InboxHasMessages_ShouldSaveFilesInReceiveDirectory()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithCertificate);

		await this.fixture.Receive();

		fixture.VerifyReceiveDirectoryIsNotEmpty();
	}

	[Test]
	public async Task Send_MissingCertificate_ShouldThrowException()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithCertificate);
		fixture.AddFileFromUnkownMarketpartnerToSendDirectory();

		var exception = Assert.ThrowsAsync<AggregateException>(() => this.fixture.Send());

		Assert.That(exception, Is.Not.Null);
		Assert.That(exception!.InnerExceptions, Is.Not.Empty);
		Assert.That(exception!.InnerExceptions[0].Message, Is.EqualTo($"No certificate found for the market partner with identification number {TestData.MarketpartnerIdWithoutCertificate}."));

		fixture.VerifySendDirectoryContainsMsconsFile();
	}

	[Test]
	[Ignore("mp logic logs this use case instead of throwing Exception")]
	public async Task Receive_MissingCertificate_ShouldThrowException()
	{
		fixture.SetupWithMarketpartner(TestData.MarketpartnerIdWithoutCertificate);

		var exception = Assert.ThrowsAsync<AggregateException>(() => this.fixture.Receive());

		Assert.That(exception, Is.Not.Null);
		Assert.That(exception!.InnerExceptions, Is.Not.Empty);
		Assert.That(exception!.InnerExceptions[0].Message, Is.EqualTo($"No certificate found for the market partner with identification number {TestData.MarketpartnerIdWithoutCertificate}."));

		fixture.VerifyReceiveDirectoryIsEmpty();
	}

	public void Dispose()
	{
		this.fixture?.Dispose();
	}
}