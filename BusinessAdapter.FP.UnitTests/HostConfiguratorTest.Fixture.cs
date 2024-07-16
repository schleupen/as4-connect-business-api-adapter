namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests;

using System;
using Moq;
using NUnit.Framework;


public partial class HostConfiguratorTest
{
	private Fixture fixture = default!;

	[SetUp]
	public void Setup()
	{
		fixture = new Fixture();
	}

	[TearDown]
	public void Dispose()
	{
		fixture = null!;
	}

	private sealed class Fixture
	{
		public Mocks Mocks { get; } = new();

		public TestData Data { get; } = new();

		public Fixture()
		{

		}
	}

	private sealed class Mocks
	{
		//public Mock<IInterface> MyMock { get; } = new();
	}

	private sealed class TestData
	{
		public string TestValue = "dummy";
	}
}