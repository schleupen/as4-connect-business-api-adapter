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

	public static string ComputeHashFor(Stream stream)
	{
		using var sha256 = System.Security.Cryptography.SHA256.Create();
		var bytes = sha256.ComputeHash(stream);

		return Convert.ToBase64String(bytes);
	}


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

	// TODO: Testcase hash mismatch
	// TODO: Testcase content mismatch
	public string WriteInboxMessage(InboxFpMessage fpMessage, string receiveDirectoryPath)
	{
		var fileName = fileNameExtractor.ExtractFileName(fpMessage);

		var filePath = Path.Combine(receiveDirectoryPath, fileName.ToFileName());
		if (File.Exists(filePath))
		{
			throw new InvalidOperationException($"File '{filePath}' already exists for message with {fpMessage.MessageId}.");
		}

		string stringResult;

		using (var xmlStream = new StreamWriter(File.Open(filePath, FileMode.CreateNew)))
		{
			using (var compressedStream = new MemoryStream(fpMessage.Payload))
			using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var resultStream = new MemoryStream())
			{
				var hashForFile = ComputeHashFor(compressedStream);

				if (fpMessage.ContentHashSha256 != hashForFile)
				{
					throw new InvalidOperationException($"Hash mismatch: file '{fileName}' [{hashForFile}] vs message '{fpMessage.MessageId}' [{fpMessage.ContentHashSha256}].");
				}
				compressedStream.Seek(0, SeekOrigin.Begin);
				zipStream.CopyTo(resultStream);
				stringResult = Encoding.UTF8.GetString(resultStream.ToArray());
				xmlStream.Write(stringResult );
			}
		}

		if (!File.Exists(filePath))
		{
			var exception = new InvalidOperationException($"Unable to write message '{fpMessage.MessageId}' payload to file '{fileName}'.");
			throw exception;
		}

		using (var fileReader = new StreamReader(File.Open(filePath, FileMode.Open)))
		{
			if (stringResult != fileReader.ReadToEnd())
			{
				throw new InvalidOperationException($"File '{fileName}' written to directory differs to received file with id '{fpMessage.MessageId}'.");
			}
		}

		return filePath;
	}
}