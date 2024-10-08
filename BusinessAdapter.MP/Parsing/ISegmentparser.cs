﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing
{
	public interface ISegmentparser
	{
		string Segmentname { get; }

		ParseSegmentResult Parse(int segmentNumber, EdifactParserConfiguration configuration, string[] composites);
	}
}
