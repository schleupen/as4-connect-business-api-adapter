namespace Schleupen.AS4.BusinessAdapter.FP;

using  Schleupen.AS4.BusinessAdapter.FP.Sending;

public interface IFpFile
{
    string? SenderIdentificationNumber { get; }

    string Path { get; }

    FpOutboxMessage CreateOutboxMessage();
}