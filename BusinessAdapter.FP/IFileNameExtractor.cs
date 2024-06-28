namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Parsing;
using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public interface IFileNameExtractor
{
    FpFileName ExtractFileName(InboxFpMessage mpMessage);
}