// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

public sealed class FileSystemWrapper : IFileSystemWrapper
{
	public string GetFileName(string path)
	{
		return Path.GetFileName(path);
	}

	public Stream OpenFileStream(string path)
	{
		return new FileStream(path, FileMode.Open);
	}
}