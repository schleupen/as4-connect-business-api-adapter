// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	[TestFixture]
	internal sealed partial class As4MessageTest
	{
		private readonly Fixture fixture = new();

		[Test]
		public void Ctor_ShouldInitializeProperties()
		{
			As4Message testObject = fixture.CreateTestObject();

			fixture.ValidateProperties(testObject);
		}
	}
}
