// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Provides access to the market partner certificates.
	/// </summary>
	public sealed class MarketpartnerCertificateProvider : IMarketpartnerCertificateProvider
	{
		private readonly ICertificateStoreFactory certificateStoreFactory;

		public MarketpartnerCertificateProvider(ICertificateStoreFactory certificateStoreFactory)
		{
			this.certificateStoreFactory = certificateStoreFactory;
		}

		/// <summary>
		/// Returns the certificate for the market partner with the given identification number.
		/// </summary>
		/// <param name="marketpartnerIdentificationNumber">The identification number of the market partner</param>
		/// <returns>The certificate.</returns>
		public IAs4Certificate GetMarketpartnerCertificate(string marketpartnerIdentificationNumber)
		{
			using (ICertificateStore store = certificateStoreFactory.CreateAndOpen())
			{
				List<IAs4Certificate> candidates = store.As4Certificates.Where(certificate => certificate.IsCertificateFor(marketpartnerIdentificationNumber)).ToList();

				if (candidates.Count > 1)
				{
					throw new NoUniqueCertificateException(marketpartnerIdentificationNumber);
				}

				IAs4Certificate? certificate = candidates.SingleOrDefault();
				if (certificate == null)
				{
					throw new MissingCertificateException(marketpartnerIdentificationNumber);
				}

				return certificate;
			}
		}
	}
}