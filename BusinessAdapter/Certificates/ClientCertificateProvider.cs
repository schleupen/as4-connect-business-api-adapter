// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Provides access to the market partner certificates.
	/// </summary>
	public sealed class ClientCertificateProvider(ICertificateStoreFactory certificateStoreFactory) : IClientCertificateProvider
	{
		/// <summary>
		/// Returns the certificate for the market partner with the given identification number.
		/// </summary>
		/// <param name="marketpartnerIdentificationNumber">The identification number of the market partner</param>
		/// <returns>The certificate.</returns>
		public IClientCertificate GetCertificate(string marketpartnerIdentificationNumber)
		{
			using (IClientCertificateStore store = certificateStoreFactory.CreateAndOpen())
			{
				List<IClientCertificate> candidates = store.Certificates.Where(certificate => certificate.IsCertificateFor(marketpartnerIdentificationNumber)).ToList();

				if (candidates.Count > 1)
				{
					throw new NoUniqueCertificateException(marketpartnerIdentificationNumber);
				}

				IClientCertificate? certificate = candidates.SingleOrDefault();
				if (certificate == null)
				{
					throw new MissingCertificateException(marketpartnerIdentificationNumber);
				}

				return certificate;
			}
		}
	}
}