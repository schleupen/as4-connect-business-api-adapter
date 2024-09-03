namespace Schleupen.AS4.BusinessAdapter.API;

using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using Schleupen.AS4.BusinessAdapter.Certificates;
using Schleupen.AS4.BusinessAdapter.Configuration;

public class HttpClientFactory(IOptions<AdapterOptions> options, IClientCertificateProvider clientCertificateProvider) : IHttpClientFactory
{
	public HttpClient CreateFor(Party party)
	{
		var certificate = clientCertificateProvider.GetCertificate(party.Id);
		
#pragma warning disable CA5386 // Hartcodierung des SecurityProtocolType-Werts vermeiden
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore CA5386 // Hartcodierung des SecurityProtocolType-Werts vermeiden
#pragma warning disable CA2000 // handler gets disposed by HttpClient.Dispose()
		return new HttpClient(CreateHttpClientHandler(certificate))
#pragma warning restore CA2000
		{
			BaseAddress = new Uri(options.Value.As4ConnectEndpoint)
		};
	}

	private static HttpClientHandler CreateHttpClientHandler(IClientCertificate clientCertificate)
	{
#pragma warning disable CA5398 // Avoid hardcoding SslProtocols values
		HttpClientHandler httpClientHandler = new HttpClientHandler
		{
			DefaultProxyCredentials = null,
			UseCookies = false,
			ClientCertificateOptions = ClientCertificateOption.Manual,
			AutomaticDecompression = DecompressionMethods.None,
			UseProxy = false,
			Proxy = null,
			PreAuthenticate = false,
			UseDefaultCredentials = false,
			Credentials = null,
			AllowAutoRedirect = false,
			SslProtocols = SslProtocols.Tls12,
			//ClientCertificates = { clientMarketpartner.Certificate.AsX509Certificate() },
			CheckCertificateRevocationList = false
		};
#pragma warning restore CA5398 // Avoid hardcoding SslProtocols values

		httpClientHandler.ClientCertificates.Add(clientCertificate.AsX509Certificate());
		httpClientHandler.ServerCertificateCustomValidationCallback = Test;

		return httpClientHandler;
	}

	private static bool Test(HttpRequestMessage arg1, X509Certificate2? arg2, X509Chain? arg3, SslPolicyErrors arg4)
	{
		// TODO ServerCertificateCustomValidationCallback returns always true?
		return true;
	}
}