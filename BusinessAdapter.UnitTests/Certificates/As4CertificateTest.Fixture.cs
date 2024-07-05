// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter.Certificates
{
	using System.Reflection;
	using System.Security.Cryptography.X509Certificates;

	internal sealed partial class ClientCertificateTest
	{
		private sealed class Fixture : IDisposable
		{
			private X509Certificate2? certificate;

			public ClientCertificate CreateTestObject()
			{
				return new ClientCertificate(ReadCertificateFromResource("client.pfx"));
			}

			private X509Certificate2 ReadCertificateFromResource(string filename)
			{
				if (certificate != null)
				{
					return certificate;
				}

				Assembly assembly = Assembly.GetExecutingAssembly();
				string resourceName = $"Schleupen.AS4.BusinessAdapter.Certificates.Resources.{filename}";

				using (Stream? fileStream = assembly.GetManifestResourceStream(resourceName))
				{
					if (fileStream == null)
					{
						throw new InvalidOperationException("Stream was null");
					}

					byte[] buffer = new byte[fileStream.Length];
					int _ = fileStream.Read(buffer, 0, buffer.Length);
					certificate = new X509Certificate2(buffer);
					return certificate;
				}
			}

			public void Dispose()
			{
				certificate?.Dispose();
			}
		}
	}
}
