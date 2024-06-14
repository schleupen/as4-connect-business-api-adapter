﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Sending
{
	using NUnit.Framework;
	using Schleupen.AS4.BusinessAdapter.MP.Sending;

	[TestFixture]
	internal sealed partial class OutboxMessageTest
	{
		private readonly Fixture fixture = new Fixture();

		[Test]
		public void Ctor_ShouldSetProperties()
		{
			OutboxMessage testObject = fixture.CreateTestObject();

			fixture.ValidateProperties(testObject);
		}
	}
}
