namespace Schleupen.AS4.BusinessAdapter.API;

using NUnit.Framework;
using Schleupen.AS4.BusinessAdapter.API.Assemblers;

public class PartyIdTypeAssemblerTest
{
	[Test]
	[TestCase("")]
	[TestCase("null")]
	[TestCase("invalid")]
	public void ToPartyTypeDto_InvalidVales_ShouldThrow(string value)
	{
		PartyIdTypeAssembler assembler = new PartyIdTypeAssembler();

		Assert.Throws<NotSupportedException>(() => assembler.ToPartyTypeDto(value));
	}

	[Test]
	[TestCase("BDEW", PartyIdTypeDto.BDEW)]
	[TestCase("DVGW", PartyIdTypeDto.DVGW)]
	[TestCase("GS1", PartyIdTypeDto.GS1)]
	[TestCase("GS1GERMANY", PartyIdTypeDto.GS1)]
	public void ToPartyTypeDto_ValidValues_ShouldAssemble(string value, PartyIdTypeDto type)
	{
		PartyIdTypeAssembler assembler = new PartyIdTypeAssembler();

		var partyType = assembler.ToPartyTypeDto(value);

		Assert.That(partyType, Is.EqualTo(type));
	}
}