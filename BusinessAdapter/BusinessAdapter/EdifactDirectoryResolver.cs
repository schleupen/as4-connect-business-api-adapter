// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.IO;
	using System.Linq;
	using Schleupen.AS4.BusinessAdapter.Parsing;
	using Schleupen.AS4.BusinessAdapter.Receiving;

	public sealed class EdifactDirectoryResolver : IEdifactDirectoryResolver
	{
		public ReadOnlyCollection<IEdifactFile> GetEditfactFilesFrom(string path)
		{
			string[] files = Directory.GetFiles(path);
			List<IEdifactFile> edifactFiles = new List<IEdifactFile>(files.Select(file => new EdifactFile(file)));

			return edifactFiles.AsReadOnly();
		}

		public void DeleteFile(string edifactFilePath)
		{
			File.Delete(edifactFilePath);
		}

		public string StoreEdifactFileTo(InboxMessage message, string receiveDirectoryPath)
		{
			IEdifactHeaderinformationParser metadataParser = new EdifactHeaderinformationParser();
			using (MemoryStream stream = new MemoryStream())
			{
				using (StreamWriter writer = new StreamWriter(stream, EdifactFile.DefaultEncoding))
				{
					writer.Write(message.EdifactContent);
					writer.Flush();
					stream.Position = 0;
					metadataParser.Parse(stream);
				}
			}

			string date = string.IsNullOrEmpty(message.BdewDocumentDate) ? message.CreatedAt.ToString("yyyymmdd") : message.BdewDocumentDate.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
			string fileName = $"{metadataParser.GetDataformatname()}_{metadataParser.GetApplicationReference()}_{message.Sender.Id}_{message.Receiver.Id}_{date}_{metadataParser.GetDocumentnumber()}.txt";
			string messagePath = Path.Combine(receiveDirectoryPath, fileName);
			using (StreamWriter edifactStream = new StreamWriter(File.Open(messagePath, FileMode.Create), EdifactFile.DefaultEncoding))
			{
				edifactStream.Write(message.EdifactContent);
			}

			return messagePath;
		}
	}
}
