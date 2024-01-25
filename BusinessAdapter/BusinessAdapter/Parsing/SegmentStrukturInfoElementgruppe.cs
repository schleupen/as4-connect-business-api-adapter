// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	public class SegmentStrukturInfoElementgruppe
	{
		private readonly int position;

		public SegmentStrukturInfoElementgruppe(int position, string name)
		{
			this.position = position;
			Name = name;
		}

		public string Name { get; }

		public bool MatchesPosition(int positionToMatch)
		{
			return position == positionToMatch;
		}
	}
}