// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Security.Cryptography.X509Certificates;

	internal sealed partial class CertificateFieldExtensionsTest
	{
		private sealed class Fixture : IDisposable
		{
			private readonly List<X509Certificate2> certificates = [];

			public X509Certificate2 ReadCertificateFromResource(string filename)
			{
				var fileName = $"Schleupen.AS4.BusinessAdapter.Certificates.Resources.{filename}";

				using Stream? fileStream = Assembly
					.GetExecutingAssembly()
					.GetManifestResourceStream(fileName);

				if (fileStream == null)
				{
					throw new InvalidOperationException($"embedded resouce not found: '{fileName}'");
				}

				byte[] buffer = new byte[fileStream.Length];
				int _ = fileStream.Read(buffer, 0, buffer.Length);
				var cert = new X509Certificate2(buffer);
				certificates.Add(cert);
				return cert;
			}

			public void Dispose()
			{
				foreach (var certificate in certificates)
				{
					certificate?.Dispose();
				}
			}
		}
	}
}