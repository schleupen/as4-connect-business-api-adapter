namespace Schleupen.AS4.BusinessAdapter.FP;

public record FpParty(string Id, string Type, string FpType, string Bilanzkreis) : Party(Id, Type)
{
    public string AsKey()
    {
        return $"{Id}@{Type}";
    }
}