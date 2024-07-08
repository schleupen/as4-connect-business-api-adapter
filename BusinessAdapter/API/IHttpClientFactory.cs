namespace Schleupen.AS4.BusinessAdapter.API;

public interface IHttpClientFactory
{
	HttpClient CreateHttpClientFor(Party party);
}