// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System;

	/// <summary>
	/// Exception that occurs when an AS4 market partner certificate is missing.
	/// </summary>
	[Serializable]
	public class MissingCertificateException : Exception
	{
		public MissingCertificateException(string marketpartnerIdentificationNumber)
			: base($"No certificate found for the market partner with identification number {marketpartnerIdentificationNumber}.")
		{
			MarketpartnerIdentificationNumber = marketpartnerIdentificationNumber;
		}

		/// <summary>
		/// Identification number for which the certificate is missing.
		/// </summary>
		public string MarketpartnerIdentificationNumber { get; }
	}
}
