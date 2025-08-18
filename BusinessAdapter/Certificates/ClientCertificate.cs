// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Security.Cryptography.X509Certificates;
	using Microsoft.IdentityModel.Tokens;

	/// <inheritdoc/>
	public class ClientCertificate(X509Certificate2 x509Certificate2) : IClientCertificate
	{
		private const string RsaVersionString = "1.2.840.113549.1.1.1";
		private const string EccVersionString = "1.2.840.10045.2.1";

		/// <inheritdoc/>
		public X509Certificate AsX509Certificate()
		{
			return x509Certificate2;
		}

		/// <inheritdoc/>
		public byte[] GetRawCertData()
		{
			return x509Certificate2.GetRawCertData();
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public bool IsCertificateFor(string marketpartnerIdentificationNumber)
		{
			string? identificationNumberFromCertificate = x509Certificate2.ResolveMarketpartnerIdentificationNumber();
				return string.Equals(identificationNumberFromCertificate, marketpartnerIdentificationNumber, StringComparison.OrdinalIgnoreCase);
		}

		/// <inheritdoc/>
		public DateTime ValidFrom => x509Certificate2.NotBefore;

		/// <inheritdoc/>
		public DateTime ValidUntil => x509Certificate2.NotAfter;
	}
}