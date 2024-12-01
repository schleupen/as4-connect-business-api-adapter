namespace Schleupen.AS4.BusinessAdapter.FP.Configuration;

public class EICMapping : Dictionary<string, List<EICMappingEntry>>
{
    public const string SectionName = nameof(EICMapping);

    public Party? GetPartyOrDefault(string identificationNumber)
    {
        var kvp = this.SelectMany(kvp => kvp.Value,
                (kvp, entry) => new { Key = kvp.Key, Entry = entry })
            .Where(kvp => kvp.Key == identificationNumber)
            .Select(kvp => new KeyValuePair<string, EICMappingEntry>(kvp.Key, kvp.Entry))
            .ToList();

        var entry = kvp.FirstOrDefault(); // TODO
        return entry.Key == null ? null : ToFpParty(entry.Value, entry.Key);
    }

    public Party? GetPartyOrDefault(EIC eic)
    {
        foreach (var entry in this)
        {
            var matchingEntry = entry.Value
                .FirstOrDefault(mappingEntry => mappingEntry.EIC == eic.Code);

            if (matchingEntry != null)
            {
                return ToFpParty(matchingEntry, entry.Key);
            }
        }

        return null;
    }

    public SendingParty GetSendingParty(EIC eic)
    {
        var party = GetPartyOrDefault(eic) ??
                    throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
        return ToSendingParty(party);
    }

    public ReceivingParty GetReceivingParty(EIC eic)
    {
        var party = GetPartyOrDefault(eic) ??
                    throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
        return ToReceivingParty(party);
    }

    private SendingParty ToSendingParty(Party sendingParty)
    {
        return new SendingParty(sendingParty.Id, sendingParty.Type);
    }

    private ReceivingParty ToReceivingParty(Party receivingParty)
    {
        return new ReceivingParty(receivingParty.Id, receivingParty.Type);
    }

    private Party ToFpParty(EICMappingEntry entry, string mpId)
    {
        return new Party(mpId, entry.MarktpartnerTyp);
    }
}