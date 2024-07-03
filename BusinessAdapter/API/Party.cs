namespace Schleupen.AS4.BusinessAdapter.API;

public record Party(string id, string type)
{
	/// <summary>
	/// The identification number.
	/// </summary>
	public string Id { get; } = id;

	/// <summary>
	/// Code providing authority of the identification e.g. BDEW.
	/// </summary>
	public string Type { get; } = type;

	public string AsKey()
	{
		return $"{id}@{type}";
	}
}