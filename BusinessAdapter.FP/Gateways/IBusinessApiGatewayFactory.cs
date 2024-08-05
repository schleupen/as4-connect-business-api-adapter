// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.FP.Gateways
{
	using Schleupen.AS4.BusinessAdapter.Certificates;

	/// <summary>
	/// Factory for business API clients to access AS4 Connect.
	/// </summary>
	public interface IBusinessApiGatewayFactory
	{
		/// <summary>
		/// Creates a IAs4BusinessApiClient to access AS4.
		/// </summary>
		/// <param name="party">Identification the own market partner which calls AS4 Connect.</param>
		/// <returns>IAs4BusinessApiClient</returns>
		/// <exception cref="MissingCertificateException">if no certificate für die angegebene Codenummer gefunden wurde.</exception>
		IBusinessApiGateway CreateGateway(FpParty party);
	}
}
