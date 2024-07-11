namespace Schleupen.AS4.BusinessAdapter.FP;


public interface IFpFileRepository
{
	Task<List<FpFile>> GetFilesFromAsync(string directory, CancellationToken cancellationToken);
	Task DeleteFileAsync(string filePath);
}

