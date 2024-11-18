namespace Schleupen.AS4.BusinessAdapter.FP;

using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;
using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public class FpFileRepository(
	IFpFileParser parser,
	IFpFileNameExtractor fileNameExtractor,
	ILogger<FpFileRepository> logger) : IFpFileRepository
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
		IEnumerable<FpFile?> fpFiles = filesInDirectory.Select(fp =>
		{
			try
			{
				var fpFile = parser.ParseFile(fp);
				return fpFile;
			}
			catch (Exception e)
			{
				logger.LogWarning(e, "could not parse in '{FilePath}'", fp);
				failedFiles.Add(new FailedFile(path, e));
				return null;
			}
		}).Where(x => x is not null).ToList();

		logger.LogInformation("found '{ValidFileCount}' valid and '{InvalidFileCount}' invalid files in '{Directory}'", fpFiles.Count(),
			failedFiles.Count, path);

		return new DirectoryResult(path, fpFiles!, failedFiles.ToList());
	}

	public void DeleteFile(string filePath)
	{
		var exists = File.Exists(filePath);
		if (!exists)
		{
			logger.LogWarning("file '{FilePath}' does not exists.", filePath);
		}

		File.Delete(filePath);
		logger.LogInformation("file '{FilePath}' removed", filePath);

	}

	// TODO missing unittests
	public string StoreXmlFileTo(InboxFpMessage fpMessage, string receiveDirectoryPath)
	{
		var fileName = fileNameExtractor.ExtractFileName(fpMessage);
		var finalFileName = fileName.ToFileName();
		string messagePath = Path.Combine(receiveDirectoryPath, finalFileName);
		if (File.Exists(messagePath))
		{
			logger.LogWarning("file '{MessagePath}' already exists and will be overriden.", messagePath);
		}

		using (StreamWriter xmlStream = new StreamWriter(File.Open(messagePath, FileMode.Create)))
		{
			using (var compressedStream = new MemoryStream(fpMessage.Payload))
			using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var resultStream = new MemoryStream())
			{
				zipStream.CopyTo(resultStream);
				xmlStream.Write(Encoding.UTF8.GetString(resultStream.ToArray()));
			}
		}

		return messagePath;
	}
}