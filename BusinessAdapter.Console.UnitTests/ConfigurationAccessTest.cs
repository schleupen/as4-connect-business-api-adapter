// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.Configuration;
using Schleupen.AS4.BusinessAdapter.MP.Configuration;

internal sealed partial class ConfigurationAccessTest : IDisposable
{
	private Fixture? fixture;

	[SetUp]
	public void Setup()
	{
		fixture = new Fixture();
	}

	[TearDown]
	public void Dispose()
	{
		fixture?.Dispose();
		fixture = null;
	}

	[Test]
	public void GetCertificateStoreLocation_WithInvalidValue_ShouldThrowCatastrophicException()
	{
		fixture!.PrepareAdapterOptions(x => x.CertificateStoreLocation = "Invalid");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		CatastrophicException? exception = Assert.Throws<CatastrophicException>(() => testObject.GetCertificateStoreLocation());

		Assert.That(exception?.Message, Is.EqualTo("Could not parse certificate store location. Please use one of the store locations available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storelocation?view=net-8.0."));
	}

	[Test]
	public void GetCertificateStoreLocation_WithValidValue_ShouldReturnStoreLocation()
	{
		fixture!.PrepareAdapterOptions(x => x.CertificateStoreLocation = "CurrentUser");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		StoreLocation location = testObject.GetCertificateStoreLocation();

		Assert.That(location, Is.EqualTo(StoreLocation.CurrentUser));
	}

	[Test]
	public void GetCertificateStoreName_WithInvalidValue_ShouldThrowCatastrophicException()
	{
		fixture!.PrepareAdapterOptions(x => x.CertificateStoreName = "Invalid");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		CatastrophicException? exception = Assert.Throws<CatastrophicException>(() => testObject.GetCertificateStoreName());

		Assert.That(exception?.Message, Is.EqualTo("Could not parse certificate store name. Please use one of the store names available at: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.storename?view=net-8.0."));
	}

	[Test]
	public void GetCertificateStoreName_WithValidValue_ShouldReturnStoreLocation()
	{
		fixture!.PrepareAdapterOptions(x => x.CertificateStoreName = "My");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		StoreName name = testObject.GetCertificateStoreName();

		Assert.That(name, Is.EqualTo(StoreName.My));
	}

	[Test]
	public void ReadOwnMarketpartners_ShouldReturnConfiguredMarketpartners()
	{
		fixture!.PrepareAdapterOptions(x => x.Marketpartners = ["12345", "54321"]);
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		IReadOnlyCollection<string> marketpartners = testObject.ReadOwnMarketpartners();

		Assert.That(marketpartners, Contains.Item("12345"));
		Assert.That(marketpartners, Contains.Item("54321"));
	}

	[Test]
	public void ReadSendDirectory_ShouldReturnSendDirectory()
	{
		fixture!.PrepareAdapterOptions(x => x.SendDirectory = @"C:\Temp\SomeSendDirectory");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		string directory = testObject.ReadSendDirectory();

		Assert.That(directory, Is.EqualTo(@"C:\Temp\SomeSendDirectory"));
	}

	[Test]
	public void ReadReceiveDirectory_ShouldReturnReceiveDirectory()
	{
		fixture!.PrepareAdapterOptions(x => x.ReceiveDirectory = @"C:\Temp\SomeReceiveDirectory");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		string directory = testObject.ReadReceiveDirectory();

		Assert.That(directory, Is.EqualTo(@"C:\Temp\SomeReceiveDirectory"));
	}

	[Test]
	public void ResolveBusinessApiEndpoint_ShouldReturnEndpoint()
	{
		fixture!.PrepareAdapterOptions(x => x.As4ConnectEndpoint = "https://some-endpoint.de/that/we/want/to/call");
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		string directory = testObject.ResolveBusinessApiEndpoint();

		Assert.That(directory, Is.EqualTo("https://some-endpoint.de/that/we/want/to/call"));
	}

	[Test]
	public void ReadAdapterConfigurationValue_ShouldReturnAdapterConfiguration()
	{
		fixture!.PrepareAdapterOptions(x =>
		{
			x.DeliveryMessageLimitCount = 1;
			x.DeliveryRetryCount = 2;
			x.ReceivingMessageLimitCount = 3;
			x.ReceivingRetryCount = 4;
		});
		ConfigurationAccess testObject = fixture!.CreateTestObject();

		AdapterConfiguration adapterConfiguration = testObject.ReadAdapterConfigurationValue();

		Assert.That(adapterConfiguration.DeliveryMessageLimitCount, Is.EqualTo(1));
		Assert.That(adapterConfiguration.DeliveryRetryCount, Is.EqualTo(2));
		Assert.That(adapterConfiguration.ReceivingMessageLimitCount, Is.EqualTo(3));
		Assert.That(adapterConfiguration.ReceivingRetryCount, Is.EqualTo(4));
	}
}