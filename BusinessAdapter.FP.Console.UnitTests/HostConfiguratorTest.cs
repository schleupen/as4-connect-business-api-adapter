// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.FP;

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