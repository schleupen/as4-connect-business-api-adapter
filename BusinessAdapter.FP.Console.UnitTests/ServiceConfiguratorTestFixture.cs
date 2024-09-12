// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Schleupen.AS4.BusinessAdapter.FP;

internal sealed partial class ServiceConfiguratorTest
{
	private sealed class Fixture
	{
		public ServiceConfigurator CreateTestObject()
		{
			return new ServiceConfigurator();
		}
	}
}