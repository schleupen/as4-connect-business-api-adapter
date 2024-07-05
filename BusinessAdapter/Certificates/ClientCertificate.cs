// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.IdentityModel.Tokens;

	/// <summary>
	/// An AS4 certificate.
	/// </summary>
	public class ClientCertificate(X509Certificate2 x509Certificate2) : IClientCertificate
	{
		private const string RsaVersionString = "1.2.840.113549.1.1.1";
		private const string EccVersionString = "1.2.840.10045.2.1";

		/// <summary>
		/// Returns the AS4 certificate as a X509 certificate.
		/// </summary>
		/// <returns>The X509 certificate</returns>
		public X509Certificate AsX509Certificate()
		{
			return x509Certificate2;
		}

		/// <summary>
		/// Returns the raw certificate data.
		/// </summary>
		/// <returns>The raw certificate data.</returns>
		public byte[] GetRawCertData()
		{
			return x509Certificate2.GetRawCertData();
		}

		/// <summary>
		/// Provides the private key.
		/// </summary>
		/// <returns>The private key.</returns>
		/// <exception cref="SecurityTokenEncryptionKeyNotFoundException">if the OID of the public key matches neither the RSA nor the ECC version string.</exception>
		public SecurityKey GetPrivateSecurityKey()
		{
			switch (x509Certificate2.PublicKey.Oid.Value)
			{
				case RsaVersionString:
					return new RsaSecurityKey(x509Certificate2.GetRSAPrivateKey());
				case EccVersionString:
					return new ECDsaSecurityKey(x509Certificate2.GetECDsaPrivateKey());
			}

			throw new SecurityTokenEncryptionKeyNotFoundException($"The private key could not be resolved for the market partner with the identification number '{x509Certificate2.IssuerName}'.");
		}

		/// <summary>
		/// Returns whether the AS4 certificate is a certificate for the given market partner.
		/// </summary>
		/// <param name="marketpartnerIdentificationNumber">The identification number of the market partner.</param>
		/// <returns>Whether the AS4 certificate belongs to the given market partner.</returns>
		public bool IsCertificateFor(string marketpartnerIdentificationNumber)
		{
			string? identificationNumberFromCertificate = ResolveFromOrganizationalUnitField();
			return string.Equals(identificationNumberFromCertificate, marketpartnerIdentificationNumber, StringComparison.OrdinalIgnoreCase);
		}

		private string? ResolveFromOrganizationalUnitField()
		{
			return x509Certificate2.ResolveFromOrganizationalUnitField();
		}
	}
}