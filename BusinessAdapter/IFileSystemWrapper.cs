// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

public interface IFileSystemWrapper
{
	string GetFileName(string path);

	Stream OpenFileStream(string path);
}