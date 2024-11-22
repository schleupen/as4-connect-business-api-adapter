namespace Schleupen.AS4.BusinessAdapter.FP;

public sealed record ReceivingFpParty(string Id, string Type)
    : FpParty(Id, Type)
{
}