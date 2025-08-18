// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.Extensions.Logging;

	/// <summary>
	/// Provides access to the market partner certificates.
	/// </summary>
	public sealed class ClientCertificateProvider(ICertificateStoreFactory certificateStoreFactory, ILogger<ClientCertificateProvider> logger) : IClientCertificateProvider
	{
		/// <summary>
		/// Returns the certificate for the market partner with the given identification number.
		/// </summary>
		/// <param name="marketpartnerIdentificationNumber">The identification number of the market partner</param>
		/// <returns>The certificate.</returns>
		public IClientCertificate GetCertificate(string marketpartnerIdentificationNumber)
		{
			using IClientCertificateStore store = certificateStoreFactory.CreateAndOpen();
			List<IClientCertificate> candidates = store.Certificates.Where(certificate => certificate.IsCertificateFor(marketpartnerIdentificationNumber)).ToList();

			var dtNow = DateTime.UtcNow;
			IClientCertificate? certificate = candidates
				.Where(z => z.ValidFrom <= dtNow && z.ValidUntil >= dtNow)
				.OrderByDescending(z => z.ValidFrom)
				.FirstOrDefault();

			if (certificate == null)
			{
				throw new MissingCertificateException(marketpartnerIdentificationNumber);
			}
			logger.LogInformation("Das Zertifikat mit dem Gültigkeitsbereich [{ValidFrom} - {ValidUntil}] wird verwendet.", certificate.ValidFrom, certificate.ValidUntil);

			return certificate;
		}
	}
}