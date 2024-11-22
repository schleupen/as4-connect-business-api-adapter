namespace Schleupen.AS4.BusinessAdapter.FP;

public sealed record SendingFpParty(string Id, string Type)
    : FpParty(Id, Type)
{
}