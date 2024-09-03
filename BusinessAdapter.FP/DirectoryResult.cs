namespace Schleupen.AS4.BusinessAdapter.FP;

public record DirectoryResult(string DirectoryPath, IEnumerable<FpFile> ValidFpFiles, IEnumerable<FailedFile> FailedFiles)
{
	public static DirectoryResult Empty(string directoryPath) => new DirectoryResult(directoryPath, [], []);

	public int TotalFileCount => ValidFpFiles.Count() + FailedFiles.Count();
}