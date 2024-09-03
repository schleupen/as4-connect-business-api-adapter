namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Configuration;
using NUnit.Framework;

public partial class SendOptionsValidatorTest
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
		public SendOptions CreateValidAdapterOptions()
		{
			var sendOptions = new SendOptions();
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.unittests.json")
				.Build();

			var options = config.GetSection(SendOptions.SendSectionName);
			options.Bind(sendOptions);

			return sendOptions;
		}
	}
}