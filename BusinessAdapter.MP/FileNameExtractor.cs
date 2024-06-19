// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP;

using Schleupen.AS4.BusinessAdapter.MP.Parsing;
using Schleupen.AS4.BusinessAdapter.MP.Receiving;

public sealed class EdifactFileNameExtractor : IEdifactFileNameExtractor
{
	public string ExtractFilename(InboxMpMessage mpMessage)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			using (StreamWriter writer = new StreamWriter(stream, EdifactFileParser.DefaultEncoding))
			{
				writer.Write(mpMessage.EdifactContent);
				writer.Flush();
				stream.Position = 0;

				IEdifactHeaderinformationParser metadataParser = new EdifactHeaderinformationParser();
				metadataParser.Parse(stream);

				string date = string.IsNullOrEmpty(mpMessage.BdewDocumentDate) ? mpMessage.CreatedAt.ToString("yyyymmdd") : mpMessage.BdewDocumentDate.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
				string fileName = $"{metadataParser.GetDataformatname()}_{metadataParser.GetApplicationReference()}_{mpMessage.Sender.Id}_{mpMessage.Receiver.Id}_{date}_{metadataParser.GetDocumentnumber()}.txt";
				return fileName;
			}
		}
	}
}