// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System;
	using System.Net;

	/// <summary>
	/// Encapsulates the result of an API call and allows to see if the call was successful.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public class BusinessApiResponse<TMessage>(bool wasSuccessful, TMessage message)
	{
		public BusinessApiResponse(bool wasSuccessful, TMessage message, HttpStatusCode responseStatusCode, Exception apiException)
		: this(wasSuccessful, message)
		{
			ResponseStatusCode = responseStatusCode;
			ApiException = apiException;
		}

		/// <summary>
		/// Returns whether the call was successful.
		/// </summary>
		public bool WasSuccessful { get; } = wasSuccessful;

		/// <summary>
		/// The message provided by the result of the call.
		/// </summary>
		public TMessage Message { get; } = message;

		/// <summary>
		/// The HTTP response code of the result.
		/// </summary>
		public HttpStatusCode? ResponseStatusCode { get; }

		/// <summary>
		/// The exception that was encountered.
		/// </summary>
		public Exception? ApiException { get; }

		/// <summary>
		/// Returns whether the response had a Too Many Requests (429) status code.
		/// </summary>
		/// <returns></returns>
		public bool HasTooManyRequestsStatusCode()
		{
			return ResponseStatusCode == HttpStatusCode.TooManyRequests;
		}
	}
}
