namespace Schleupen.AS4.BusinessAdapter.FP;

using NUnit.Framework;

public sealed partial class SendAndReceiveTests : IDisposable
{
    [Test]
    public async Task SendAndReceiveTests_SendOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

		var sendStatus = await this.fixture.Send(configFileOption);

		Assert.That(sendStatus.FailedMessages.Count, Is.Zero);
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(1));

		// we expect the file to be gone after sending
		Assert.That(fixture.CheckSendFile(), Is.False);
    }

    [Test]
    public async Task SendAndReceiveTests_ReceiveOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var receiveStatus = await this.fixture.Receive(configFileOption);

	    Assert.That(receiveStatus.FailedMessages.Count, Is.Zero);
	    Assert.That(receiveStatus.SuccessfulMessages.Count, Is.EqualTo(4));

	    // we expect files to be downloaded in the configured receive directory
	    Assert.That(fixture.CheckReceiveFileDirIsNotEmpty(), Is.True);
	}

    [Test]
    public async Task SendAndReceiveTests_ReceiveOfValidFiles_WithoutCert()
    {
	    fixture.CreateDefaultAppSettings("9912345000003");
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var exception = Assert.ThrowsAsync<AggregateException> (() => this.fixture.Receive(configFileOption));
	    Assert.That(exception.InnerExceptions[0].Message, Is.EqualTo("No certificate found for the market partner with identification number 9912345000003."));

	    // we expect the file to be there after a failing send
	    Assert.That(fixture.CheckSendFile(), Is.True);
    }

    [Test]
    public async Task SendAndReceiveTests_ReceiveOfValidFiles_NoMappingForId()
    {
	    fixture.CreateDefaultAppSettings("9912345000010");
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var exception = Assert.ThrowsAsync<AggregateException> (() => this.fixture.Receive(configFileOption));

	    Assert.That(exception.InnerExceptions[0].Message, Is.EqualTo("Receiving party 9912345000010 mapping not configured"));
	    // we expect the file to be there after a failing send
	    Assert.That(fixture.CheckSendFile(), Is.True);
    }

	public void Dispose()
	{
		this.fixture?.Dispose();
	}
}