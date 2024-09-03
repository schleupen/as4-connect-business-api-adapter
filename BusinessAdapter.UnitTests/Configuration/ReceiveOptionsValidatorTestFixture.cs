namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Configuration;
using NUnit.Framework;

public partial class ReceiveOptionsValidatorTest
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
		public TestData Data { get; } = new();

		public Fixture()
		{
			SetupDirectories();
		}

		private void SetupDirectories()
		{
			Directory.CreateDirectory("./send");
			Directory.CreateDirectory("./receive");
		}
	}

	private sealed class TestData
	{
		public ReceiveOptions CreateValidAdapterOptions()
		{
			var options = new ReceiveOptions();
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.unittests.json")
				.Build();

			var section = config.GetSection(ReceiveOptions.ReceiveSectionName);
			section.Bind(options);

			return options;
		}
	}
}