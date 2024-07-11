namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using NUnit.Framework;

public partial class HostConfiguratorTest
{
	[Test]
	public void ConfigureHost_ShouldNotThrow()
	{
		HostConfigurator configurator = new HostConfigurator();
		configurator.ConfigureHost(Array.Empty<string>());
	}
}