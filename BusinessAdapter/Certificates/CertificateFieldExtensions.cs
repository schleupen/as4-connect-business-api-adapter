// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;

	/// <summary>
	/// Extension methods for working with certificates.
	/// </summary>
	public static class CertificateFieldExtensions
	{
		private const string OrganizationalUnitField = "OU";
		private const string DnQualifierField = "dnQualifier";
		private const string As4DistinguishedName = "AS4";
		private static readonly string[] SubjectSeparators = { ",", "=" };

		/// <summary>
		/// Resolves the market partner based on the organizational unit field of the given certificate.
		/// </summary>
		/// <param name="certificate">The certificate.</param>
		/// <returns>The market partner.</returns>
		public static string? ResolveFromOrganizationalUnitField(this X509Certificate2 certificate)
		{
			if (string.IsNullOrEmpty(certificate.Subject))
			{
				return null;
			}

			List<string> subjectParts = certificate.Subject.Split(SubjectSeparators, StringSplitOptions.RemoveEmptyEntries).Select(y => y.Trim()).ToList();
			int ouIndex = subjectParts.IndexOf(OrganizationalUnitField);
			return ouIndex == -1
				? null
				: ParseIdentificationNumberFromOuField(subjectParts[ouIndex + 1]);
		}

		/// <summary>
		/// Returns whether the subject distinguished name of the certificate is AS4.
		/// </summary>
		/// <param name="certificate">The certificate.</param>
		/// <returns>True if the distinguished name is AS4.</returns>
		public static bool IsSubjectDistinguishedNameEqualToAs4(this X509Certificate2 certificate)
		{
			if (string.IsNullOrEmpty(certificate.Subject))
			{
				return false;
			}

			List<string> subjectParts = certificate.Subject.Split(SubjectSeparators, StringSplitOptions.RemoveEmptyEntries).Select(y => y.Trim()).ToList();
			int dnIndex = subjectParts.IndexOf(DnQualifierField);
			return dnIndex != -1 && subjectParts[dnIndex + 1].Equals(As4DistinguishedName, StringComparison.OrdinalIgnoreCase);
		}

		private static string? ParseIdentificationNumberFromOuField(string organizationalUnitOrIssuerField)
		{
			if (string.IsNullOrEmpty(organizationalUnitOrIssuerField))
			{
				throw new ArgumentNullException(nameof(organizationalUnitOrIssuerField));
			}

			string[] splitField = organizationalUnitOrIssuerField.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
			return splitField.Length < 2 ? null : splitField[0];
		}
	}
}
