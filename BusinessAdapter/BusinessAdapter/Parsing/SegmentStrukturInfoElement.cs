// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	public class SegmentStrukturInfoElement
	{
		private readonly int position;
		private readonly SegmentStrukturInfoElementgruppe? gruppe;
		private readonly Elementtyp? elementTyp;


		public SegmentStrukturInfoElement(int position, string name, SegmentStrukturInfoElementgruppe? gruppe, Elementtyp? elementTyp)
		{
			this.position = position;
			Name = name;
			this.gruppe = gruppe;
			this.elementTyp = elementTyp;
		}

		public string Name { get; }

		public bool MatchesPosition(int positionToMatch)
		{
			if (gruppe == null)
			{
				return position == positionToMatch;
			}

			return false;
		}

		public bool MatchesPosition(int positionToMatch, int gruppendatenelementPosition)
		{
			if (gruppe == null)
			{
				return false;
			}

			return gruppe.MatchesPosition(positionToMatch) && position == gruppendatenelementPosition;
		}

		public enum Elementtyp
		{
			Alphanumerisch,
			Numerisch,
		}

		public bool IsNumeric()
		{
			return elementTyp == Elementtyp.Numerisch;
		}
	}
}