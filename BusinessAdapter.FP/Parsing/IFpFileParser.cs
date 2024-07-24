namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

public interface IFpFileParser
{
    FpFile Parse(string path);

    FpParsedPayload ParsePayload(byte[] payload);
}