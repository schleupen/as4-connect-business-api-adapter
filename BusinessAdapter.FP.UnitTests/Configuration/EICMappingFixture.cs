namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Configuration;

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
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
				{ Data.Party1.Id, Data.MappingParty1 },
				{ Data.Party2.Id, Data.MappingParty2 }
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
		public readonly Party Party1 = new Party("id1", "type");
		public readonly Party Party2 = new Party("id2", "type");

		public readonly List<EICMappingEntry> MappingParty1 =
		[
			new()
			{
				EIC = "eic1",
				MarktpartnerTyp = "type"
			}
		];

		public readonly List<EICMappingEntry> MappingParty2 =
		[
			new()
			{
				EIC = "eic2",
				MarktpartnerTyp = "type"
			}
		];
	}
}