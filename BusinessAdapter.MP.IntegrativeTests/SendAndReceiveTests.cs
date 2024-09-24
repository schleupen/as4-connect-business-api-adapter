﻿namespace Schleupen.AS4.BusinessAdapter.MP;

using NUnit.Framework;

public sealed partial class SendAndReceiveTests : IDisposable
{
    [Test]
    public async Task SendAndReceiveTests_SendOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

		await this.fixture.Send(configFileOption);

		// we expect the file to be gone after sending
		Assert.That(fixture.CheckSendFile(), Is.False);
    }

    [Test]
    public async Task SendAndReceiveTests_ReceiveOfValidFiles()
    {
	    fixture.CreateDefaultAppSettings();
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    await this.fixture.Receive(configFileOption);

	    // we expect files to be downloaded in the configured receive directory
	    Assert.That(fixture.CheckReceiveFileDirIsNotEmpty(), Is.True);
	}

    [Test]
    public async Task SendAndReceiveTests_SendOfValidFiles_WithoutCert()
    {
	    fixture.CreateDefaultAppSettings("9912345000003");
	    var configFileOption = new FileInfo(fixture.AppSettingsPath);

	    var exception = Assert.ThrowsAsync<AggregateException> (() => this.fixture.Receive(configFileOption));
	    Assert.That(exception.InnerExceptions[0].Message, Is.EqualTo("No certificate found for the market partner with identification number 9912345000003."));


	    // we expect the file to be there after a failing send
	    Assert.That(fixture.CheckSendFile(), Is.True);
    }

    public void Dispose()
    {
	    this.fixture?.Dispose();
    }
}