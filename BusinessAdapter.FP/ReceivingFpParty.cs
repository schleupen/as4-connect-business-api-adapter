namespace Schleupen.AS4.BusinessAdapter.FP;

public sealed record ReceivingFpParty(string Id, string Type, string FpType, string Bilanzkreis) 
    : FpParty(Id, Type, FpType, Bilanzkreis)
{
}