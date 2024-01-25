// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System.Net.Http;
	using BusinessApi;

	/// <summary>
	/// Factory for the client wrapper
	/// </summary>
	public sealed class ClientWrapperFactory : IClientWrapperFactory
	{
		/// <summary>
		/// Creates a client wrapper that uses the given endpoint and HTTP client.
		/// </summary>
		/// <param name="as4BusinessApiEndpoint">The AS4 Connect endpoint.</param>
		/// <param name="httpClient">The HTTP client to use.</param>
		/// <returns>The configured client wrapper.</returns>
		public IClientWrapper Create(string as4BusinessApiEndpoint, HttpClient httpClient)
		{
			return new ClientWrapper(new Client(as4BusinessApiEndpoint, httpClient));
		}
	}
}