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
				{ Data.Eic1.Code, Data.MappingParty1 },
				{ Data.Eic2.Code, Data.MappingParty2 }
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
		public readonly FpParty Party1 = new FpParty("id1", "type", "fpTyp", "Bilanzkreis");
		public readonly FpParty Party2 = new FpParty("id2", "type", "fpTyp", "Bilanzkreis");

		public readonly List<EICMappingEntry> MappingParty1 = new List<EICMappingEntry>()
		{
			new EICMappingEntry()
			{
				Bilanzkreis = "Bilanzkreis",
				EIC = "eic1",
				FahrplanHaendlerTyp = "fpTyp",
				MarktpartnerTyp = "type"
			}
		};

		public readonly List<EICMappingEntry> MappingParty2 = new List<EICMappingEntry>()
		{
			new EICMappingEntry()
			{
				Bilanzkreis = "Bilanzkreis",
				EIC = "eic2",
				FahrplanHaendlerTyp = "fpTyp",
				MarktpartnerTyp = "type"
			}
		};
	}
}