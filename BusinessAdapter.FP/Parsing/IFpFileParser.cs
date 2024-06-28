namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

public interface IFpFileParser
{
    IFpFile Parse(string path);
}