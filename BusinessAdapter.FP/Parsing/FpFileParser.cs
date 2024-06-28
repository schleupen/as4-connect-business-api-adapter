namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Xml.Linq;

public class FpFileParser(IFileSystemWrapper fileSystemWrapper) : IFpFileParser
{
	private readonly string ESS_NAMESPACE_STRING = "urn:entsoe.eu:wgedi:ess";

    public IFpFile Parse(string path)
    {
         string filename = fileSystemWrapper.GetFileName(path);

         XDocument doc = XDocument.Load(path);

         var parser = this.CreateParserFor(doc);
         return parser.Parse(doc, filename, path);
    }

    private IFpFileSpecificParser CreateParserFor(XDocument doc)
    {
	    XNamespace? ns = doc.Root?.GetDefaultNamespace();

	    // TODO maybe find a better way to determine the format
	    if (ns!.NamespaceName.Contains(ESS_NAMESPACE_STRING))
	    {
		    return new EssFileParser();
	    }
	    else
	    {
		    return new CimFileParser();
	    }
    }
}