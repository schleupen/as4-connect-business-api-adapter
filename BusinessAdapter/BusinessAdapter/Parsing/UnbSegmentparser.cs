// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	public class UnbSegmentparser : AbstractSegmentparser
	{
		public UnbSegmentparser()
		{
			SegmentStrukturInfo = BuildSegmentStrukturInfo();
		}

		protected override SegmentStrukturInfo SegmentStrukturInfo { get; }

		private static SegmentStrukturInfo BuildSegmentStrukturInfo()
		{
			return new SegmentStrukturInfo()
				.StartDataelementgroup("S001")
					.AddDataElement("0001")
					.AddDataElement("0002")
				.CloseDataElementGroup()
				.StartDataelementgroup("S002")
					.AddDataElement("0004")
					.AddDataElement("0007")
					.AddDataElement("0008")
				.CloseDataElementGroup()
				.StartDataelementgroup("S003")
					.AddDataElement("0010")
					.AddDataElement("0007")
					.AddDataElement("0014")
				.CloseDataElementGroup()
				.StartDataelementgroup("S004")
					.AddDataElement("0017")
					.AddDataElement("0019")
				.CloseDataElementGroup()
				.AddDataElement("0020")
				.StartDataelementgroup("S005")
					.AddDataElement("0022")
					.AddDataElement("0025")
				.CloseDataElementGroup()
				.AddDataElement("0026")
				.AddDataElement("0029")
				.AddDataElement("0031")
				.AddDataElement("0032")
				.AddDataElement("0035")
				.Build();
		}
	}
}