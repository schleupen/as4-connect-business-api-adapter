namespace Schleupen.AS4.BusinessAdapter.API.Assemblers;

public interface IPartyIdTypeAssembler
{
	PartyIdTypeDto ToPartyTypeDto(string partyTypeValue);
}