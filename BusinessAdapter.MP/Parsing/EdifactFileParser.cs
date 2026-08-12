// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.MP.Parsing;

using System;
using System.IO;
using System.Text;

public class EdifactFileParser(IFileSystemWrapper fileSystemWrapper) : IEdifactFileParser
{
	public static readonly Encoding DefaultEncoding = EdifactEncoding.GetEncoding();

	private IEdifactHeaderinformationParser? metadataParser;

	public IEdifactFile Parse(string path)
	{
		using (Stream stream = fileSystemWrapper.OpenFileStream(path))
		{
			byte[]? content;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				content = memoryStream.ToArray();
				stream.Position = 0;
			}

			metadataParser = new EdifactHeaderinformationParser();
			metadataParser.Parse(stream);

			string filename = fileSystemWrapper.GetFileName(path);

			string? senderIdentificationNumber = metadataParser.GetAbsenderCodenummer()
				?? throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
			string? receiverIdentificationNumber = metadataParser.GetEmpfaengerCodenummer()
				?? throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
			string? receiverIdentificationNumberType = Enum.Parse<CodeVergebendeStelle>(metadataParser.GetReceiverIdentificationNumberType().ToString()).ToString()
				?? throw new ArgumentException($"Could not retrieve receiver code number type from file {path}.");
			string? documentNumber = metadataParser.GetDocumentnumber()
				?? throw new ArgumentException($"Could not retrieve document number from file {path}.");
			string? dataformatname = metadataParser.GetDataformatname()
				?? throw new ArgumentException($"Could not retrieve data format name from file {path}.");
			DateTimeOffset documentDate = metadataParser.GetErstellungszeitpunkt();
			byte[]? payload = content
				?? throw new ArgumentException($"Could not retrieve the payload from file {path}.");

			return new EdifactFile(path, filename, receiverIdentificationNumber, senderIdentificationNumber, receiverIdentificationNumberType, dataformatname, documentNumber, documentDate, payload);
		}
	}
}