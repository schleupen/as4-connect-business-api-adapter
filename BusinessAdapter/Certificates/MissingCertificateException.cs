// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System;

	/// <summary>
	/// Exception that occurs when an AS4 market partner certificate is missing.
	/// </summary>
	[Serializable]
	public class MissingCertificateException(string marketpartnerIdentificationNumber)
		: Exception($"No certificate found for the market partner with identification number {marketpartnerIdentificationNumber}.")
	{
		/// <summary>
		/// Identification number for which the certificate is missing.
		/// </summary>
		public string MarketpartnerIdentificationNumber { get; } = marketpartnerIdentificationNumber;
	}
}
