namespace Schleupen.AS4.BusinessAdapter.FP;

[Serializable]
public class FileAlreadyExistException(string fileName, string messageId)
    : Exception($"File with the name {fileName} already exist for message {messageId}")
{
    public string FileName { get; } = fileName;

    public string MessageId { get; } = messageId;
}