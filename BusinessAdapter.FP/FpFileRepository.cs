namespace Schleupen.AS4.BusinessAdapter.FP;

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;

// TODO Unittest
public class FpFileRepository(IFpFileParser parser, ILogger<FpFileRepository> logger) : IFpFileRepository
{
	public IImmutableList<FpFile> GetFilesFrom(string path)
	{
		var result =  Directory.GetFiles(path)
			.Select(parser.Parse)
			.ToImmutableList();

		logger.LogInformation("found '{FileCount}' files in '{Directory}'", result.Count, path);

		return result;
	}

	public void DeleteFile(string filePath)
	{
		File.Delete(filePath);
	}
}