namespace Schleupen.AS4.BusinessAdapter.FP.UnitTests.Configuration;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API;

public partial class EICMappingTest
{
	[Test]
	public void GetEICOrDefault_Mapped_ShouldReturnEic()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		var result = mapping.GetEICOrDefault(fixture.Data.Party1);

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.EqualTo(fixture.Data.Eic1));
	}

	[Test]
	public void GetEICOrDefault_Unmapped_ShouldReturnNull()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		var result = mapping.GetEICOrDefault(new Party("na", "na"));

		Assert.That(result, Is.Null);
	}

	[Test]
	public void GetEIC_Unmapped_ShouldThrow()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		Assert.Throws<InvalidOperationException>(() => mapping.GetEIC(new Party("na", "na")));
	}

	[Test]
	public void GetPartyOrDefault_Mapped_ShouldReturnParty()
	{
		var mapping = fixture.CreateSimpleEicMapping();

		var result = mapping.GetPartyOrDefault(fixture.Data.Eic1);

		Assert.That(result, Is.EqualTo(fixture.Data.Party1));
	}

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
		Assert.That(eicMapping.GetPartyOrDefault(new EIC("10Y1001A1001A82H")), Is.EqualTo(new Party("1000000001", "BDEW")));
	}

	[Test]
	public void GetSendingParty()
	{
		var eicMapping = fixture.CreateSimpleEicMapping();
		var sendingParty = eicMapping.GetSendingParty(fixture.Data.Eic1);

		Assert.That(sendingParty.Id, Is.EqualTo(fixture.Data.Party1.Id));
		Assert.That(sendingParty.Type, Is.EqualTo(fixture.Data.Party1.Type));
	}

	[Test]
	public void GetReceivingParty()
	{
		var eicMapping = fixture.CreateSimpleEicMapping();
		var sendingParty = eicMapping.GetReceivingParty(fixture.Data.Eic2);

		Assert.That(sendingParty.Id, Is.EqualTo(fixture.Data.Party2.Id));
		Assert.That(sendingParty.Type, Is.EqualTo(fixture.Data.Party2.Type));
	}
}