﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	using System.Net.Http;

	/// <summary>
	/// Factory for the client wrapper
	/// </summary>
	public sealed class BusinessApiClientFactory : IBusinessApiClientFactory
	{
		/// <summary>
		/// Creates a client wrapper that uses the given endpoint and HTTP client.
		/// </summary>
		/// <param name="businessApiEndpoint">The AS4 Connect endpoint.</param>
		/// <param name="httpClient">The HTTP client to use.</param>
		/// <returns>The configured client wrapper.</returns>
		public IBusinessApiClient Create(Uri businessApiEndpoint, HttpClient httpClient)
		{
			return new BusinessApiClient(businessApiEndpoint.AbsoluteUri, httpClient);
		}
	}
}