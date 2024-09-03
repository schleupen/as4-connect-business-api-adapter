// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing
{
	public class EdifactDatenelementgruppe
	{
		private readonly int position;

		public EdifactDatenelementgruppe(int position)
		{
			this.position = position;
		}

		public bool MatchesPosition(int positionToMatch)
		{
			return position == positionToMatch;
		}

		public SegmentStrukturInfoElement GetDataElementInfo(SegmentStrukturInfo segmentStrukturInfo, int datengruppenelementPosition)
		{
			return segmentStrukturInfo.GetDataElementInfo(position, datengruppenelementPosition);
		}
	}
}