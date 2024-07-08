namespace Schleupen.AS4.BusinessAdapter.API.Assemblers;

// TODO Test
public class PartyIdTypeAssembler : IPartyIdTypeAssembler
{
	public PartyIdTypeDto ToPartyTypeDto(string partyTypeValue)
	{
		var partyType = partyTypeValue.ToUpperInvariant();
		switch (partyType)
		{
			case "BDEW":
				return PartyIdTypeDto.BDEW;
			case "DVGW":
				return PartyIdTypeDto.DVGW;
			case "GS1":
			case "GS1GERMANY":
				return PartyIdTypeDto.GS1;
			default:
				throw new NotSupportedException($"PartyType '{partyType}' is unsupported");
		}
	}
}