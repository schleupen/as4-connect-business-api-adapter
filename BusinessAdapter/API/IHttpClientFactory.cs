namespace Schleupen.AS4.BusinessAdapter.API;

public interface IHttpClientFactory
{
	HttpClient CreateFor(Party party);
}