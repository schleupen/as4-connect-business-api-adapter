// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing;

public interface IEdifactFileParser
{
	IEdifactFile Parse(string path);
}