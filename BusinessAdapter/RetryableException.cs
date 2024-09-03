// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter
{
	/// <summary>
	/// A retryable exception.
	/// </summary>
	public sealed class RetryableException(string message, Exception innerException) : Exception(message, innerException);
}