// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.API
{
	/// <summary>
	/// Factory for client wrappers.
	/// </summary>
	public interface IBusinessApiClientFactory
	{
		IBusinessApiClient Create(Uri uri, HttpClient httpClient);
	}
}