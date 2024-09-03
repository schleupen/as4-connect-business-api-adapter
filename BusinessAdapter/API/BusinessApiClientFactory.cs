// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Factory for <see cref="IBusinessApiClient"/>
	/// </summary>
	public sealed class BusinessApiClientFactory : IBusinessApiClientFactory
	{
		/// <inheritdoc />
		public IBusinessApiClient Create(Uri uri, HttpClient httpClient)
		{
			return new BusinessApiClient(uri.AbsoluteUri, httpClient);
		}
	}
}