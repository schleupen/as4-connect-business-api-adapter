namespace Schleupen.AS4.BusinessAdapter.FP.Configuration;

public class EICMapping : Dictionary<string, List<EICMappingEntry>>
{
    public const string SectionName = nameof(EICMapping);

    public EIC? GetEICForEic(string eicCode)
    {
        var entry = this.SelectMany(kvp => kvp.Value)
            .FirstOrDefault(entry => entry.EIC == eicCode);
        return entry == null ? null : new EIC(entry.EIC);
    }

    public EIC? GetEICForMpId(string mpId)
    {
        var entry = this.FirstOrDefault(x => x.Key == mpId).Value?.FirstOrDefault();
        return entry == null ? null : new EIC(entry.EIC);
    }

    public FpParty? GetParty(string identifcationNumber)
    {
        var kvp = this.SelectMany(kvp => kvp.Value,
                (kvp, entry) => new { Key = kvp.Key, Entry = entry })
            .Where(kvp => kvp.Key == identifcationNumber)
            .Select(kvp => new KeyValuePair<string, EICMappingEntry>(kvp.Key, kvp.Entry))
            .ToList();

        var entry = kvp.FirstOrDefault();
        return entry.Key == null ? null : ToFpParty(entry.Value, entry.Key);
    }

    public FpParty? GetPartyOrDefault(EIC eic)
    {
        var entry = this.SelectMany(kvp => kvp.Value, (kvp, entry) => new { Key = kvp.Key, Entry = entry })
            .FirstOrDefault(kvp => kvp.Entry.EIC == eic.Code);

        return entry == null ? null : ToFpParty(entry.Entry, entry.Key);
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

    private FpParty ToFpParty(EICMappingEntry entry, string mpId)
    {
        return new FpParty(mpId, entry.MarktpartnerTyp, entry.FahrplanHaendlerTyp, entry.Bilanzkreis);
    }
}