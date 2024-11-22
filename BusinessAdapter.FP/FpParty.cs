namespace Schleupen.AS4.BusinessAdapter.FP;

public record FpParty(string Id, string Type) : Party(Id, Type)
{
    public string AsKey()
    {
        return $"{Id}@{Type}";
    }
}