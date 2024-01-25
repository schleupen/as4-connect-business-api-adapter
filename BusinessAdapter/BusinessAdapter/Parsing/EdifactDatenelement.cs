// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	public class EdifactDatenelement
	{
		private readonly string? wert;
		private readonly int position;
		private readonly EdifactDatenelementgruppe? datenelementgruppe;

		public EdifactDatenelement(string? wert, int position, EdifactDatenelementgruppe? datenelementgruppe)
		{
			this.wert = wert;
			this.position = position;
			this.datenelementgruppe = datenelementgruppe;
		}

		public string? Wert => wert;

		public bool MatchesPosition(int positionToMatch)
		{
			if (datenelementgruppe == null)
			{
				return position == positionToMatch;
			}

			return false;
		}

		public bool MatchesPosition(int positionToMatch, int gruppendatenelementPosition)
		{
			if (datenelementgruppe == null)
			{
				return false;
			}

			return datenelementgruppe.MatchesPosition(positionToMatch) && position == gruppendatenelementPosition;
		}
	}
}