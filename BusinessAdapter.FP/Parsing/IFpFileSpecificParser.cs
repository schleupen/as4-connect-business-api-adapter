namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Xml.Linq;

public interface IFpFileSpecificParser
{
	IFpFile Parse(XDocument document, string filename, string path);
}