using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Schleupen.AS4.BusinessAdapter;

using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;

public partial class InboundMessagesTests
{
    private Fixture fixture = new Fixture();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        fixture = new Fixture();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        fixture?.Dispose();
    }

    private sealed class Fixture : IDisposable
    {
        private readonly ServiceProvider serviceProvider;

        public Fixture()
        {
          // serviceProvider = CreateServiceProvider();

        }

        //public TestData Data { get; } = new();

        public async Task StartAsync()
        {
            await using HostApplicationFactory<Schleupen.AS4.BusinessAdapter.FP.Program> hostApplicationFactory =
                new(configuration: builder =>
                {
                    builder.UseEnvironment("Integration");
                  //  builder.UseSetting("SomeAppSetting:Key", "replacement value");

                    builder.ConfigureTestServices(services =>
                    {
                    });
                });

            await hostApplicationFactory.RunHostAsync();
        }
        
        public void Dispose()
        {
         //   configScope.Dispose();
         //   serviceProvider.Dispose();
        }
    }
}