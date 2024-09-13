namespace Schleupen.AS4.BusinessAdapter;

using NUnit.Framework;

public partial class SendAndReceiveTests
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
    public async Task SendAndReceiveTests_SendOfValidFiles_WithoutCert()
    {
	    fixture.CreateDefaultAppSettings("9912345000003");
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var sendStatus = await this.fixture.Send(configFileOption);
		
	    Assert.That(sendStatus.FailedMessages.Count, Is.EqualTo(1));
	    Assert.That(sendStatus.SuccessfulMessages.Count, Is.EqualTo(0));
	   // Assert.That(sendStatus.FailedMessages.ToList()[0].Exception.Message, Is.EqualTo("Exception text"));

	    // we expect the file to be there after a failing send
	    Assert.That(fixture.CheckSendFile(), Is.True);
    }
}