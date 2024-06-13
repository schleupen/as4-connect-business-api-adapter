// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using System.Collections.ObjectModel;
using Schleupen.AS4.BusinessAdapter.Receiving;

public interface IEdifactDirectoryResolver
{
	ReadOnlyCollection<IEdifactFile> GetEditfactFilesFrom(string path);

	void DeleteFile(string edifactFilePath);

	string StoreEdifactFileTo(InboxMessage message, string receiveDirectoryPath);
}