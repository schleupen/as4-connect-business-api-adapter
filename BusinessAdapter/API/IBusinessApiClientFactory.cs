// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Factory for client wrappers.
	/// </summary>
	public interface IBusinessApiClientFactory
	{
		/// <summary>
		/// Creates a client wrapper with the given endpoint and HTTP client.
		/// </summary>
		/// <param name="as4BusinessApiEndpoint">The AS4 Connect endpoint.</param>
		/// <param name="httpClient">The HTTP client to use.</param>
		/// <returns>The configured client wrapper.</returns>
		IBusinessApiClient Create(string as4BusinessApiEndpoint, HttpClient httpClient);
	}
}