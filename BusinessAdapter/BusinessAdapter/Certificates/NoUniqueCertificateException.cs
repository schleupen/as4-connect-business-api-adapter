// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	/// <summary>
	/// Exception that occurs when an AS4 market partner has more than one certificate.
	/// </summary>
	[Serializable]
	public class NoUniqueCertificateException : Exception
	{
		public NoUniqueCertificateException(string marketpartnerIdentificationNumber)
			: base($"More than one certificate found for the market partner with identification number {marketpartnerIdentificationNumber}.")
		{
		}
	}
}