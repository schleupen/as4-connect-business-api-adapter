namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.IO.Compression;
using System.Text;
using System.Xml.Linq;

public class FpFileParser(IFileSystemWrapper fileSystemWrapper, IFpParsedFileValidator fpParsedFileValidator) : IFpFileParser
{
	private readonly string ESS_NAMESPACE_STRING = "urn:entsoe.eu:wgedi:ess";

    public FpFile Parse(string path)
    {
         string fileName = fileSystemWrapper.GetFileName(path);

         XDocument doc = XDocument.Load(path);

         var parser = this.CreateParserFor(doc);
         var parsedFile = parser.Parse(doc, fileName, path);
         fpParsedFileValidator.ValidateParsedFpFile(parsedFile);
		 return parsedFile;
    }

    public FpParsedPayload ParsePayload(byte[] payload)
    {
	    string xml = "";
	    using (var compressedStream = new MemoryStream(payload))
	    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
	    using (var resultStream = new MemoryStream())
	    {
		    zipStream.CopyTo(resultStream);
		    xml = Encoding.UTF8.GetString(resultStream.ToArray());

	    }
		string responseText = System.Text.Encoding.ASCII.GetString(payload);
		XDocument doc = XDocument.Parse(xml);
	    var parser = this.CreateParserFor(doc);
	    return parser.ParsePayload(doc);
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

	    if (ns!.NamespaceName.Contains(ESS_NAMESPACE_STRING))
	    {
		    return true;
	    }

	    return false;
    }
}