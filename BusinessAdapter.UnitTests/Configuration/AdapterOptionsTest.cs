namespace Schleupen.AS4.BusinessAdapter.Configuration;

using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

public class AdapterOptionsTest
{
	[Test]
	public void Bind_ShouldBindAllProperties()
	{
		var adapterOptions = new AdapterOptions();
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.unittests.json")
			.Build();

		var options = config.GetSection(AdapterOptions.SectionName);
		options.Bind(adapterOptions);

		Assert.That(adapterOptions.As4ConnectEndpoint, Is.EqualTo("https://erp.prod.as4.schleupen.cloud"));
		Assert.That(adapterOptions.CertificateStoreLocation, Is.EqualTo(StoreLocation.CurrentUser));
		Assert.That(adapterOptions.CertificateStoreName, Is.EqualTo(StoreName.CertificateAuthority));
		Assert.That(adapterOptions.Marketpartners!.Single(), Is.EqualTo("9984617000002"));

		Assert.That(adapterOptions.Receive!.Directory, Is.EqualTo("./receive"));
		Assert.That(adapterOptions.Receive.Retry.Count, Is.EqualTo(22));
		Assert.That(adapterOptions.Receive.MessageLimitCount, Is.EqualTo(22));
		Assert.That(adapterOptions.Receive.SleepDuration, Is.EqualTo(TimeSpan.FromSeconds(22)));

		Assert.That(adapterOptions.Send!.Directory, Is.EqualTo("./send"));
		Assert.That(adapterOptions.Send.MessageLimitCount, Is.EqualTo(11));
		Assert.That(adapterOptions.Send.SleepDuration, Is.EqualTo(TimeSpan.FromSeconds(11)));
		Assert.That(adapterOptions.Send.Retry.Count, Is.EqualTo(11));
		Assert.That(adapterOptions.Send.Retry.SleepDuration, Is.EqualTo(TimeSpan.FromSeconds(11)));
	}
}