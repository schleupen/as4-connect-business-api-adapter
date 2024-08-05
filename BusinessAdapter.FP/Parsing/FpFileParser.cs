namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Xml.Linq;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.FP.Configuration;

public class FpFileParser(IFileSystemWrapper fileSystemWrapper, IOptions<EICMapping> eicMapping) : IFpFileParser
{
	private readonly string ESS_NAMESPACE_STRING = "urn:entsoe.eu:wgedi:ess";

    public FpFile Parse(string path)
    {
         string fileName = fileSystemWrapper.GetFileName(path);

         XDocument doc = XDocument.Load(path);

         var parser = this.CreateParserFor(doc);
         return parser.Parse(doc, fileName, path);
    }

    public FpParsedPayload ParsePayload(byte[] payload)
    {
	    string responseText = System.Text.Encoding.ASCII.GetString(payload);
	    XDocument doc = XDocument.Parse(responseText); 
	    var parser = this.CreateParserFor(doc);
	    return parser.ParsePayload(doc);
    }

    private IFpFileSpecificParser CreateParserFor(XDocument doc)
    {
	    if (IsEssDocument(doc))
	    {
		    return new EssFileParser(eicMapping);
	    }

	    return new CimFileParser(eicMapping);
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