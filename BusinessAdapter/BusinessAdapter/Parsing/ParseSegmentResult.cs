// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Parsing
{
	using System;

	public sealed class ParseSegmentResult
	{
		public ParseSegmentResult(EdifactSegment edifactSegment)
		{
			Segment = edifactSegment ?? throw new ArgumentNullException(nameof(edifactSegment));
		}

		public EdifactSegment Segment { get; }
	}
}