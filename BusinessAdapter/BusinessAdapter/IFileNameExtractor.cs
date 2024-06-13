// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Schleupen.AS4.BusinessAdapter.Receiving;

public interface IFileNameExtractor
{
	string ExtractFilename(InboxMessage message);
}