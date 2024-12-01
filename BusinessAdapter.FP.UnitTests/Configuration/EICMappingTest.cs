namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Configuration;

using NUnit.Framework;

public partial class EICMappingTest
{
	[Test]
	public void GetPartyOrDefault_Unmapped_ShouldReturnNull()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		var result = mapping.GetPartyOrDefault(new EIC("na"));

		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetParty_Unmapped_ShouldThrow()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		Assert.Throws<InvalidOperationException>(() => mapping.GetSendingParty(new EIC("na")));
	}

	[Test]
	public void GetPartyOrDefault_EICMappingFromJson_ShouldWork()
	{
		var eicMapping = this.fixture.LoadFromAppSettings();

		Assert.That(eicMapping, Is.Not.Null.Or.Empty);
		Assert.That(eicMapping.GetPartyOrDefault(new EIC("5790000432752")), Is.EqualTo(new Party("1000000001", "BDEW")));
	}

	[Test]
	public void GetPartyOrDefault_ShouldReturnCorrectFpParty()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		Assert.That(mapping.GetPartyOrDefault(fixture.Data.Party1.Id), Is.Not.Null);
	}

	// TODO missing test cases
}