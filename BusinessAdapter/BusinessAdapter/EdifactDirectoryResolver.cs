// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Schleupen.AS4.BusinessAdapter.Receiving;

public sealed class EdifactDirectoryResolver : IEdifactDirectoryResolver
{
	private readonly IFileNameExtractor fileNameExtractor;
	private readonly IEdifactFileParser edifactFileParser;

	public EdifactDirectoryResolver(IFileNameExtractor fileNameExtractor, IEdifactFileParser edifactFileParser)
	{
		this.fileNameExtractor = fileNameExtractor;
		this.edifactFileParser = edifactFileParser;
	}

	public ReadOnlyCollection<IEdifactFile> GetEditfactFilesFrom(string path)
	{
		string[] files = Directory.GetFiles(path);
		List<IEdifactFile> edifactFiles = new List<IEdifactFile>();

		foreach (string file in files)
		{
			IEdifactFile edifactFile = edifactFileParser.Parse(file);
			edifactFiles.Add(edifactFile);
		}
		
		return edifactFiles.AsReadOnly();
	}

	public void DeleteFile(string edifactFilePath)
	{
		File.Delete(edifactFilePath);
	}

	public string StoreEdifactFileTo(InboxMessage message, string receiveDirectoryPath)
	{
		string fileName = fileNameExtractor.ExtractFilename(message);
		string messagePath = Path.Combine(receiveDirectoryPath, fileName);
		using (StreamWriter edifactStream = new StreamWriter(File.Open(messagePath, FileMode.Create), EdifactFileParser.DefaultEncoding))
		{
			edifactStream.Write(message.EdifactContent);
		}

		return messagePath;
	}
}