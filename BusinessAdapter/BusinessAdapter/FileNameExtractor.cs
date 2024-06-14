// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using Schleupen.AS4.BusinessAdapter.Parsing;
using Schleupen.AS4.BusinessAdapter.Receiving;

public sealed class FileNameExtractor : IFileNameExtractor
{
	public string ExtractFilename(InboxMessage message)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			using (StreamWriter writer = new StreamWriter(stream, EdifactFileParser.DefaultEncoding))
			{
				writer.Write(message.EdifactContent);
				writer.Flush();
				stream.Position = 0;

				IEdifactHeaderinformationParser metadataParser = new EdifactHeaderinformationParser();
				metadataParser.Parse(stream);

				string date = string.IsNullOrEmpty(message.BdewDocumentDate) ? message.CreatedAt.ToString("yyyymmdd") : message.BdewDocumentDate.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
				string fileName = $"{metadataParser.GetDataformatname()}_{metadataParser.GetApplicationReference()}_{message.Sender.Id}_{message.Receiver.Id}_{date}_{metadataParser.GetDocumentnumber()}.txt";
				return fileName;
			}
		}
	}
}