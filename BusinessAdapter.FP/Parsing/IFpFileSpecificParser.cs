namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

using System.Xml.Linq;

public interface IFpFileSpecificParser
{
	FpFile Parse(XDocument document, string filename, string path);

	FpPayloadInfo ParsePayload(XDocument document);
}