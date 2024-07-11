namespace Schleupen.AS4.BusinessAdapter.FP;

using System.Collections.Immutable;

public interface IFpFileRepository
{
	IImmutableList<FpFile> GetFilesFrom(string path);

	void DeleteFile(string filePath);
}

