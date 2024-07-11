namespace Schleupen.AS4.BusinessAdapter.FP.Configuration;

public sealed class EICMapping : Dictionary<string, Party>
{
	public const string SectionName = nameof(EICMapping);

	public EIC? GetEICOrDefault(Party party)
	{
		var partyToEic = this.ToDictionary(x => x.Value, y => y.Key);
		var eicOrNull = partyToEic.GetValueOrDefault(party);
		return eicOrNull is null ? null : new EIC(eicOrNull);
	}

	public Party? GetPartyOrDefault(EIC eic)
	{
		return this.GetValueOrDefault(eic.Code);
	}

	public EIC GetEIC(Party party)
	{
		return GetEICOrDefault(party) ?? throw new InvalidOperationException($"party '{party.AsKey()}' is not configured.");;
	}

	public SendingParty GetSendingParty(EIC eic)
	{
		var party = GetPartyOrDefault(eic) ?? throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
		return ToSendingParty(party);
	}

	public ReceivingParty GetReceivingParty(EIC eic)
	{
		var party = GetPartyOrDefault(eic) ?? throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
		return ToReceivingParty(party);
	}

	private SendingParty ToSendingParty(Party sendingParty)
	{
		return new SendingParty(sendingParty.Id, sendingParty.Id);
	}

	private ReceivingParty ToReceivingParty(Party sendingParty)
	{
		return new ReceivingParty(sendingParty.Id, sendingParty.Id);
	}
}