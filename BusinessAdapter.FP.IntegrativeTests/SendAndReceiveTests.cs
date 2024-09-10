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

		Assert.That(sendStatus.FailedMessages.Count, Is.Zero);
		Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(1));

		// TODO : Add filesystem checks
    }

    [Test]
    public async Task ReceiveOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var receiveStatus = await this.fixture.Receive(configFileOption);

	    Assert.That(receiveStatus.FailedMessages.Count, Is.Zero);
	    Assert.That(receiveStatus.SuccessfulMessages.Count, Is.EqualTo(4));

	    // TODO : Add filesystem checks
	}
}