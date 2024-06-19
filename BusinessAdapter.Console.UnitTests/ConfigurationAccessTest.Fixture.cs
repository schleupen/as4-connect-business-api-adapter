// Copyright...:  (c)  Schleupen SE

namespace Schleupen.AS4.BusinessAdapter;

using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Schleupen.AS4.BusinessAdapter.Configuration;

internal sealed partial class ConfigurationAccessTest
{
	private sealed class Fixture : IDisposable
	{
		public Mocks Mocks { get; } = new();

		public ConfigurationAccess CreateTestObject()
		{
			return new ConfigurationAccess(Mocks.AdapterOptions.Object, Mocks.Logger.Object);
		}

		public void PrepareAdapterOptions(params Action<AdapterOptions>[] configureActions)
		{
			SetupConfigurationSummary();

			Mocks.AdapterOptions
				.Setup(x => x.Value)
				.Returns(() =>
				{
					AdapterOptions options = new AdapterOptions { CertificateStoreLocation = "bla" };
					foreach (Action<AdapterOptions> action in configureActions)
					{
						action(options);
					}

					return options;
				});
		}

		private void SetupConfigurationSummary()
		{
			SetupLogger(LogLevel.Information, "Current configuration: ", _ => true);
		}

		public void Dispose()
		{
			Mocks.Dispose();
		}

		private void SetupLogger(LogLevel expectedLogLevel, string expectedMessage, Func<Exception, bool> validateException)
		{
			Func<object, Type, bool> state = (v, _) => v.ToString()!.StartsWith(expectedMessage, true, CultureInfo.InvariantCulture);

			Mocks.Logger.Setup(
				x => x.Log(
					It.Is<LogLevel>(l => l == expectedLogLevel),
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => state(v, t)),
					It.Is<Exception>(e => validateException(e)),
					It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
		}
	}

	private sealed class Mocks
	{
		public Mock<IOptions<AdapterOptions>> AdapterOptions { get; } = new(MockBehavior.Strict);

		public Mock<ILogger<ConfigurationAccess>> Logger { get; } = new(MockBehavior.Strict);

		public void Dispose()
		{
			AdapterOptions.VerifyAll();
			Logger.VerifyAll();
		}
	}
}