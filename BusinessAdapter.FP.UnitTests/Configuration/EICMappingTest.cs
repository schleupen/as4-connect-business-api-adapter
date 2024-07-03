namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Configuration;

using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public class EICMappingTest
{
	[Test]
	public void GetEICOrDefault_Mapped_ShouldReturnEic()
	{
		var mapping = CreateSimpleEicMapping();

		var result = mapping.GetEICOrDefault(new Party("id1", "type"));

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo(new EIC("eic1")));
	}

	[Test]
	public void GetEICOrDefault_Unmapped_ShouldReturnNull()
	{
		var mapping = CreateSimpleEicMapping();

		var result = mapping.GetEICOrDefault(new Party("na", "na"));

		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetPartyOrDefault_Mapped_ShouldReturnParty()
	{
		var mapping = CreateSimpleEicMapping();

		var result = mapping.GetPartyOrDefault(new EIC("eic1"));

		Assert.That(result, Is.EqualTo(new Party("id1", "type")));
	}

	[Test]
	public void GetPartyOrDefault_Unmapped_ShouldReturnNull()
	{
		var mapping = CreateSimpleEicMapping();

		var result = mapping.GetPartyOrDefault(new EIC("na"));

		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetPartyOrDefault_EICMappingFromJson_ShouldWork()
	{
		var eicMapping = this.LoadFromAppSettings();

		Assert.That(eicMapping, Is.Not.Null.Or.Empty);
		Assert.That(eicMapping.GetPartyOrDefault(new EIC("10Y1001A1001A82H")), Is.EqualTo(new Party("1000000001", "BDEW")));
	}

	private EICMapping LoadFromAppSettings()
	{
		EICMapping mapping = new EICMapping();
		var config = new ConfigurationBuilder()
			.AddJsonFile("appsettings.unittests.json")
			.Build();

		var options = config.GetSection(EICMapping.SectionName);
		options.Bind(mapping);

		return mapping;
	}

	private static EICMapping CreateSimpleEicMapping()
	{
		EICMapping mapping = new EICMapping()
		{
			{ "eic1", new Party("id1", "type") },
			{ "eic2", new Party("id2", "type") }
		};
		return mapping;
	}
}