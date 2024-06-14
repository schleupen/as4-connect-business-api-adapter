// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

public interface IEdifactFileParser
{
	IEdifactFile Parse(string path);
}