namespace Schleupen.AS4.BusinessAdapter.FP;

public interface IFpFileRepository
{
	DirectoryResult GetFilesFrom(string path);

	void DeleteFile(string filePath);
}

