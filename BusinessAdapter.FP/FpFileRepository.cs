namespace Schleupen.AS4.BusinessAdapter.FP;

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

// TODO Unittest
public class FpFileRepository(IFpFileParser parser, ILogger<FpFileRepository> logger) : IFpFileRepository
{
	public IImmutableList<FpFile> GetFilesFrom(string path)
	{
		var di = new DirectoryInfo(path);
		if (!di.Exists)
		{
			logger.LogWarning("directory '{Directory}' not found", path);
			return ImmutableList<FpFile>.Empty;
		}

		var filesInDirectory = Directory.GetFiles(path);

		var fpFiles = filesInDirectory.Select(fp =>
		{
			try
			{
				var fpFile = parser.Parse(fp);
				return fpFile;
			}
			catch (Exception e)
			{
				logger.LogWarning(e, "could not parse in '{FilePath}'", fp);
				return null;
			}
		}).Where(x => x != null).ToImmutableList();

		logger.LogInformation("found '{FileCount}' files in '{Directory}'", fpFiles.Count, path);
		return fpFiles;
	}

	public void DeleteFile(string filePath)
	{
		File.Delete(filePath);
	}
}