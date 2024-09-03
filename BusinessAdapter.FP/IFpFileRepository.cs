namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public interface IFpFileRepository
{
	DirectoryResult GetFilesFrom(string path);

	void DeleteFile(string filePath);

	string StoreXmlFileTo(InboxFpMessage fpMessage, string receiveDirectoryPath);
}

