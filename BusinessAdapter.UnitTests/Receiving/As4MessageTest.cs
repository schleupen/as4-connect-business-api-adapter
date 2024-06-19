// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Receiving
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.MP.Receiving;

	[TestFixture]
	internal sealed partial class MpMessageTest
	{
		private readonly Fixture fixture = new();

		[Test]
		public void Ctor_ShouldInitializeProperties()
		{
			MpMessage testObject = fixture.CreateTestObject();

			fixture.ValidateProperties(testObject);
		}
	}
}
