// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Schleupen.AS4.BusinessAdapter.MP.Receiving;

public interface IEdifactFileNameExtractor
{
	string ExtractFilename(InboxMessage message);
}