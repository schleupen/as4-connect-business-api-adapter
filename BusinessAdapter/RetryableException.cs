// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	/// <summary>
	/// A retryable exception.
	/// </summary>
	public sealed class RetryableException : Exception
	{
		public RetryableException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}