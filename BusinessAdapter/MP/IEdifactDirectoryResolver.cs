// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using System.Collections.ObjectModel;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;

public interface IEdifactDirectoryResolver
{
	ReadOnlyCollection<IEdifactFile> GetEditfactFilesFrom(string path);

	void DeleteFile(string edifactFilePath);

	string StoreEdifactFileTo(InboxMessage message, string receiveDirectoryPath);
}