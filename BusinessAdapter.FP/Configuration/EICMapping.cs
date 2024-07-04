﻿namespace Schleupen.AS4.BusinessAdapter.FP.Configuration;

using Schleupen.AS4.BusinessAdapter.API;

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

	public Party GetParty(EIC eic)
	{
		return GetPartyOrDefault(eic) ?? throw new InvalidOperationException($"EIC '{eic.Code}' is not configured.");
	}
}