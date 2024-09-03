// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	[TestFixture]
	internal sealed partial class MpOutboxMessageTest
	{
		private readonly Fixture fixture = new Fixture();

		[Test]
		public void Ctor_ShouldSetProperties()
		{
			MpOutboxMessage testObject = fixture.CreateTestObject();

			fixture.ValidateProperties(testObject);
		}
	}
}
