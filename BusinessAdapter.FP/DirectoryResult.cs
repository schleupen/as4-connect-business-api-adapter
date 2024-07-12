namespace Schleupen.AS4.BusinessAdapter.FP;

using System.Collections.Immutable;

public record DirectoryResult(string DirectoryPath, ImmutableList<FpFile> ValidFpFiles, ImmutableList<FailedFile> FailedFiles)
{
	public static DirectoryResult Empty(string directoryPath) => new DirectoryResult(directoryPath, ImmutableList<FpFile>.Empty, ImmutableList<FailedFile>.Empty);

	public int TotalFileCount => ValidFpFiles.Count + FailedFiles.Count;
}