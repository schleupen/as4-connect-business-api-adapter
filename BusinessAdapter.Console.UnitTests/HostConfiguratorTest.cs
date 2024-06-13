// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using NUnit.Framework;

[TestFixture]
internal sealed partial class HostConfiguratorTest
{
	private readonly Fixture fixture = new();

	[Test]
	public void ConfigureHost_ShouldConfigureHostAndBuildWithoutException()
	{
		HostConfigurator configurator = fixture.CreateTestObject();

		Assert.DoesNotThrow(() => configurator.ConfigureHost([]));
	}
}