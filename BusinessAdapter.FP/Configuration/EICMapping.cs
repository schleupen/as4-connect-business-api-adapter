namespace Schleupen.AS4.BusinessAdapter.FP.Configuration;

public sealed class EICMapping : Dictionary<string, FpParty>
{
    public const string SectionName = nameof(EICMapping);

    public EIC? GetEICOrDefault(FpParty party)
    {
        var partyToEic = this.ToDictionary(x => x.Value, y => y.Key);
        var eicOrNull = partyToEic.GetValueOrDefault(party);
        return eicOrNull is null ? null : new EIC(eicOrNull);
    }

    public EIC? GetEIC(string eicCode)
    {
        var eicCodeToEic = this.ToDictionary(x => x.Key);
        var eicOrNull = eicCodeToEic.Where(x => x.Key == eicCode).FirstOrDefault();
        return new EIC(eicOrNull.Key);
    }

    public FpParty? GetParty(string identifcationNumber)
    {
        return this.Where(i => i.Value.Id == identifcationNumber).FirstOrDefault().Value;
    }

    public FpParty? GetPartyOrDefault(EIC eic)
    {
        return this.GetValueOrDefault(eic.Code);
    }

    public EIC GetEIC(FpParty party)
    {
        return GetEICOrDefault(party) ??
               throw new InvalidOperationException($"party '{party.AsKey()}' is not configured.");
    }

    public SendingFpParty GetSendingParty(EIC eic)
    {
        var party = GetPartyOrDefault(eic) ??
                    throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
        return ToSendingParty(party);
    }

    public ReceivingFpParty GetReceivingParty(EIC eic)
    {
        var party = GetPartyOrDefault(eic) ??
                    throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
        return ToReceivingParty(party);
    }

    private SendingFpParty ToSendingParty(FpParty sendingParty)
    {
        return new SendingFpParty(sendingParty.Id, sendingParty.Type, sendingParty.FpType, sendingParty.Bilanzkreis);
    }

    private ReceivingFpParty ToReceivingParty(FpParty receivingParty)
    {
        return new ReceivingFpParty(receivingParty.Id, receivingParty.Type, receivingParty.FpType,
            receivingParty.Bilanzkreis);
    }
}