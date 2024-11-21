namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

public class FpFileParser(IFileSystemWrapper fileSystemWrapper, IFpParsedFileValidator fpParsedFileValidator)
	: IFpFileParser
{
    public FpFile ParseFile(string path)
    {
         string fileName = fileSystemWrapper.GetFileName(path);
         XDocument doc = XDocument.Load(path);

         var parser = this.CreateParserFor(doc);
         var parsedFile = parser.Parse(doc, fileName, path);
         fpParsedFileValidator.ValidateParsedFpFile(parsedFile);
		 return parsedFile;
    }

    public FpPayloadInfo ParseCompressedPayload(byte[] payload)
    {
	    using (var compressedStream = new MemoryStream(payload))
	    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
	    using (var resultStream = new MemoryStream())
	    {
		    zipStream.CopyTo(resultStream);
		    var xml = Encoding.UTF8.GetString(resultStream.ToArray());
		    XDocument doc = XDocument.Parse(xml);
		    var parser = this.CreateParserFor(doc);
		    return parser.ParsePayload(doc);
	    }
    }

    private IFpFileSpecificParser CreateParserFor(XDocument doc)
    {
	    if (IsEssDocument(doc))
	    {
		    return new EssFileParser();
	    }

	    return new CimFileParser();
    }

    private bool IsEssDocument(XDocument document)
    {
	    XNamespace? ns = document.Root?.GetDefaultNamespace();

	    if (ns!.NamespaceName.Contains(CimFileParser.XmlNamespace))
	    {
		    return false;
	    }

	    return true;
    }
}