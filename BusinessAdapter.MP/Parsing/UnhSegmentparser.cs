// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing
{
	public sealed class UnhSegmentparser : AbstractSegmentparser, ISegmentStrukturInfoBuilder
	{
		public UnhSegmentparser()
		{
			SegmentStrukturInfo = BuildSegmentStrukturInfo();
		}

		protected override SegmentStrukturInfo SegmentStrukturInfo { get; }

		SegmentStrukturInfo ISegmentStrukturInfoBuilder.BuildSegmentStrukturInfo()
		{
			return BuildSegmentStrukturInfo();
		}

		private static SegmentStrukturInfo BuildSegmentStrukturInfo()
		{
			return new SegmentStrukturInfo()
				.AddDataElement("0062")
				.StartDataelementgroup("S009")
					.AddDataElement("0065")
					.AddDataElement("0052")
					.AddDataElement("0054")
					.AddDataElement("0051")
					.AddDataElement("0057")
				.CloseDataElementGroup()
				.AddDataElement("0068")
				.StartDataelementgroup("S010")
					.AddDataElement("0070")
					.AddDataElement("0073")
				.CloseDataElementGroup()
				.Build();
		}
	}
}