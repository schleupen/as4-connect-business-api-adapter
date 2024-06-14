// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using System;
using System.IO;
using System.Text;
using Schleupen.AS4.BusinessAdapter.Parsing;

public class EdifactFileParser : IEdifactFileParser
{
	public static readonly Encoding DefaultEncoding = EdifactEncoding.GetEncoding();

	private readonly IFileSystemWrapper fileSystemWrapper;
	private IEdifactHeaderinformationParser? metadataParser;

	public EdifactFileParser(IFileSystemWrapper fileSystemWrapper)
	{
		this.fileSystemWrapper = fileSystemWrapper;
	}

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

			string? senderIdentificationNumber = metadataParser.GetAbsenderCodenummer();
			if (senderIdentificationNumber == null)
			{
				throw new ArgumentException($"Could not retrieve sender code number from file {path}.");
			}

			string? receiverIdentificationNumber = metadataParser.GetEmpfaengerCodenummer();
			if (receiverIdentificationNumber == null)
			{
				throw new ArgumentException($"Could not retrieve receiver code number from file {path}.");
			}

			string? receiverIdentificationNumberType = Enum.Parse(typeof(CodeVergebendeStelle), metadataParser.GetReceiverIdentificationNumberType().ToString()).ToString();
			if (receiverIdentificationNumberType == null)
			{
				throw new ArgumentException($"Could not retrieve receiver code number type from file {path}.");
			}
			
			string? documentNumber = metadataParser.GetDocumentnumber();
			if (documentNumber == null)
			{
				throw new ArgumentException($"Could not retrieve document number from file {path}.");
			}

			string? dataformatname = metadataParser.GetDataformatname();
			if (dataformatname == null)
			{
				throw new ArgumentException($"Could not retrieve data format name from file {path}.");
			}

			DateTimeOffset documentDate = metadataParser.GetErstellungszeitpunkt();

			byte[]? payload = content;
			if (payload == null)
			{
				throw new ArgumentException($"Could not retrieve the payload from file {path}.");
			}

			return new EdifactFile(path, filename, receiverIdentificationNumber, senderIdentificationNumber, receiverIdentificationNumberType, dataformatname, documentNumber, documentDate, payload);
		} 
	}
}