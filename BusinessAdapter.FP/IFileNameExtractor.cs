namespace Schleupen.AS4.BusinessAdapter.FP;

using Schleupen.AS4.BusinessAdapter.FP.Receiving;

public interface IFileNameExtractor
{
    string ExtractFilename(InboxFpMessage mpMessage);
}