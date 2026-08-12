namespace Schleupen.AS4.BusinessAdapter;

public static class IEnumerableExtensions
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
	{
		return enumerable == null || !enumerable.Any();
	}
}
