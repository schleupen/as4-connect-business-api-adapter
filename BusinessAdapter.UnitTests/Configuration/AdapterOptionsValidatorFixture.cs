namespace Schleupen.AS4.BusinessAdapter.Configuration;

using Microsoft.Extensions.Configuration;
using NUnit.Framework;

public partial class AdapterOptionsValidatorTest
{
	private Fixture? fixture;

	[SetUp]
	public void Setup()
	{
		fixture = new Fixture();
	}

	[TearDown]
	public void Dispose()
	{
		fixture = null;
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

	private class TestData
	{
		public AdapterOptions CreateValidAdapterOptions()
		{
			var adapterOptions = new AdapterOptions();
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.unittests.json")
				.Build();

			var options = config.GetSection(AdapterOptions.SectionName);
			options.Bind(adapterOptions);

			return adapterOptions;
		}
	}
}