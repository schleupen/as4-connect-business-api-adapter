namespace Schleupen.AS4.BusinessAdapter;

public record Party(string Id, string Type)
{
	public string AsKey()
	{
		return $"{Id}@{Type}";
	}
}