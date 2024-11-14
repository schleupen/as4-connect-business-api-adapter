namespace Schleupen.AS4.BusinessAdapter.FP.Parsing;

public interface IFpFileParser
{
    FpFile ParseFile(string path);

    FpPayloadInfo ParseCompressedPayload(byte[] payload);
}