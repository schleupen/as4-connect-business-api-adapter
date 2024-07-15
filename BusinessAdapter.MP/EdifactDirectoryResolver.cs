// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;

public sealed class EdifactDirectoryResolver(IEdifactFileNameExtractor fileNameExtractor, IEdifactFileParser edifactFileParser)
	: IEdifactDirectoryResolver
{
	private readonly IEdifactFileNameExtractor fileNameExtractor = fileNameExtractor;
	private readonly IEdifactFileParser edifactFileParser = edifactFileParser;

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

	public string StoreEdifactFileTo(InboxMpMessage mpMessage, string receiveDirectoryPath)
	{
		string fileName = fileNameExtractor.ExtractFilename(mpMessage);
		string messagePath = Path.Combine(receiveDirectoryPath, fileName);
		using (StreamWriter edifactStream = new StreamWriter(File.Open(messagePath, FileMode.Create), EdifactFileParser.DefaultEncoding))
		{
			edifactStream.Write(mpMessage.EdifactContent);
		}

		return messagePath;
	}
}