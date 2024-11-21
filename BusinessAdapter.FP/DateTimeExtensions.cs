namespace Schleupen.AS4.BusinessAdapter.FP;

public static class DateTimeExtensions
{
	public static string ToHyphenDate(this DateTime value)
	{
		return value.ToUniversalTime().ToString("yyyy-MM-dd");
	}

	public static string ToFileDate(this DateTime value)
	{
		return value.ToUniversalTime().ToString("yyyyMMdd");
	}
}