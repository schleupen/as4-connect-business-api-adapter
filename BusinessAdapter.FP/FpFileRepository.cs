namespace Schleupen.AS4.BusinessAdapter.FP;

// TODO Implement
public class FpFileRepository : IFpFileRepository
{
	public Task<List<FpFile>> GetFilesFromAsync(string directory, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task DeleteFileAsync(string filePath)
	{
		throw new NotImplementedException();
	}
}