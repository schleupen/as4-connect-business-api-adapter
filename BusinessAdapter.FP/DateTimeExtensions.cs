namespace Schleupen.AS4.BusinessAdapter.FP;

public static class DateTimeExtensions
{
	public static string ToHyphenDate(this DateTime value)
	{
		return value.ToUniversalTime().ToString(DateTimeFormat.HyphenDate);
	}

	public static string ToFileDate(this DateTime value)
	{
		return value.ToUniversalTime().ToString(DateTimeFormat.FileDate);
	}

	public static string ToFileTimestamp(this DateTime value)
	{
		return value.ToUniversalTime().ToString(DateTimeFormat.FileTimestamp);
	}
}