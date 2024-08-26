namespace Schleupen.AS4.BusinessAdapter;

using NUnit.Framework;

public partial class SendAndReceiveTests
{
    [Test]
    public async Task SendOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

		var sendStatus = await this.fixture.Send(configFileOption);

		Assert.That(sendStatus.FailedMessageCount, Is.Zero);
		Assert.That(sendStatus.SuccessfulMessageCount, Is.EqualTo(1));
    }

    [Test]
    public async Task ReceiveOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var receiveStatus = await this.fixture.Receive(configFileOption);

	    Assert.That(receiveStatus.FailedMessages, Is.Zero);
	    Assert.That(receiveStatus.SuccessfulMessages, Is.EqualTo(4));
	}
}