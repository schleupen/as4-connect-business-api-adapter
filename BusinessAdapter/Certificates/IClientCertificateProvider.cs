﻿// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	/// <summary>
	/// Provider for market partner certificates.
	/// </summary>
	public interface IClientCertificateProvider
	{
		/// <summary>
		/// Returns the client certificate for the given identification number.
		/// </summary>
		/// <param name="marketpartnerIdentificationNumber">The identification number of the market partner.</param>
		/// <returns>The marketpartner with certificate.</returns>
		IClientCertificate GetCertificate(string marketpartnerIdentificationNumber);
	}
}
