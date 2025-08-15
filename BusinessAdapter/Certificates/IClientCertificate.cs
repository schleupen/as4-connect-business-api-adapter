// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.IdentityModel.Tokens;

	/// <summary>
	/// An AS4 certificate.
	/// </summary>
	public interface IClientCertificate
	{
		/// <summary>
		/// Returns the AS4 certificate as a X509 certificate.
		/// </summary>
		/// <returns>The X509 certificate</returns>
		X509Certificate AsX509Certificate();

		/// <summary>
		/// Returns the raw certificate data.
		/// </summary>
		/// <returns>The raw certificate data.</returns>
		byte[] GetRawCertData();

		/// <summary>
		/// Provides the private key.
		/// </summary>
		/// <returns>The private key.</returns>
		/// <exception cref="SecurityTokenEncryptionKeyNotFoundException">if the OID of the public key matches neither the RSA nor the ECC version string.</exception>
		SecurityKey GetPrivateSecurityKey();

		/// <summary>
		/// Returns whether the AS4 certificate is a certificate for the given market partner.
		/// </summary>
		/// <param name="marketpartnerIdentificationNumber">The identification number of the market partner.</param>
		/// <returns>Whether the AS4 certificate belongs to the given market partner.</returns>
		bool IsCertificateFor(string marketpartnerIdentificationNumber);

		/// <summary>
		/// Returns the time stamp from which the certificate is valid
		/// </summary>
		/// <returns>The Datetime</returns>
		DateTime ValidFrom { get; }

		/// <summary>
		/// Returns the time stamp until the certificate is valid
		/// </summary>
		/// <returns>The Datetime </returns>
		DateTime ValidUntil { get; }
	}
}
