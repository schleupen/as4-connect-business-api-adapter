namespace Schleupen.AS4.BusinessAdapter.Configuration;

using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

public class AdapterOptionsTest
{
	[Test]
	public void Bind_AdapterOptions_ShouldBindAllProperties()
	{
		var adapterOptions = new AdapterOptions();
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.unittests.json")
			.Build();

		var adapterSection = config.GetSection(AdapterOptions.SectionName);
		adapterSection.Bind(adapterOptions);

		Assert.That(adapterOptions.As4ConnectEndpoint, Is.EqualTo("https://erp.prod.as4.schleupen.cloud"));
		Assert.That(adapterOptions.CertificateStoreLocation, Is.EqualTo(StoreLocation.CurrentUser));
		Assert.That(adapterOptions.CertificateStoreName, Is.EqualTo(StoreName.CertificateAuthority));
		Assert.That(adapterOptions.Marketpartners!.Single(), Is.EqualTo("9984617000002"));
	}

	[Test]
	public void Bind_SendOptions_ShouldBindAllProperties()
	{
		var sendOptions = new SendOptions();
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.unittests.json")
			.Build();

		var adapterSection = config.GetSection(SendOptions.SendSectionName);
		adapterSection.Bind(sendOptions);

		Assert.That(sendOptions.Directory, Is.EqualTo("./send"));
		Assert.That(sendOptions.MessageLimitCount, Is.EqualTo(11));
		Assert.That(sendOptions.SleepDuration, Is.EqualTo(TimeSpan.FromSeconds(11)));
		Assert.That(sendOptions.Retry.Count, Is.EqualTo(11));
		Assert.That(sendOptions.Retry.SleepDuration, Is.EqualTo(TimeSpan.FromSeconds(11)));
	}

	[Test]
	public void Bind_ReceiveOptions_ShouldBindAllProperties()
	{
		var options = new ReceiveOptions();
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.unittests.json")
			.Build();

		var adapterSection = config.GetSection(ReceiveOptions.ReceiveSectionName);
		adapterSection.Bind(options);

		Assert.That(options.Directory, Is.EqualTo("./receive"));
		Assert.That(options.Retry.Count, Is.EqualTo(22));
		Assert.That(options.MessageLimitCount, Is.EqualTo(22));
		Assert.That(options.SleepDuration, Is.EqualTo(TimeSpan.FromSeconds(22)));
	}
}