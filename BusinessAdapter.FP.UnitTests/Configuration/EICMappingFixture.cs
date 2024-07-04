namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Configuration;

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public partial class EICMappingTest
{
	private Fixture fixture = new();

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

		public EICMapping CreateSimpleEicMapping()
		{
			EICMapping mapping = new EICMapping()
			{
				{ Data.Eic1.Code, Data.Party1 },
				{ Data.Eic2.Code, Data.Party2 }
			};
			return mapping;
		}

		public EICMapping LoadFromAppSettings()
		{
			EICMapping mapping = new EICMapping();
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.unittests.json")
				.Build();

			var options = config.GetSection(EICMapping.SectionName);
			options.Bind(mapping);

			return mapping;
		}
	}

	private sealed class TestData
	{
		public readonly EIC Eic1 = new EIC("eic1");
		public readonly EIC Eic2 = new EIC("eic2");
		public readonly Party Party1 = new Party("id1", "type");
		public readonly Party Party2 = new Party("id2", "type");
	}
}