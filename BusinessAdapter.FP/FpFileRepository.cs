namespace Schleupen.AS4.BusinessAdapter.FP;

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

public class FpFileRepository(IFpFileParser parser, ILogger<FpFileRepository> logger) : IFpFileRepository
{
	public DirectoryResult GetFilesFrom(string path)
	{
		var di = new DirectoryInfo(path);
		if (!di.Exists)
		{
			logger.LogWarning("directory '{Directory}' not found", path);
			return DirectoryResult.Empty(path);
		}

		var filesInDirectory = Directory.GetFiles(path);

		List<FailedFile> failedFiles = new List<FailedFile>();
		ImmutableList<FpFile?> fpFiles = filesInDirectory.Select(fp =>
		{
			try
			{
				var fpFile = parser.Parse(fp);
				return fpFile;
			}
			catch (Exception e)
			{
				logger.LogWarning(e, "could not parse in '{FilePath}'", fp);
				failedFiles.Add(new FailedFile(path, e));
				return null;
			}
		}).Where(x => x is not null).ToImmutableList();

		logger.LogInformation("found '{ValidFileCount}' valid and '{InvalidFileCount}' invalid files in '{Directory}'", fpFiles.Count, failedFiles.Count, path);

		return new DirectoryResult(path, fpFiles!, failedFiles.ToImmutableList());
	}

	public void DeleteFile(string filePath)
	{
		File.Delete(filePath);
	}
}