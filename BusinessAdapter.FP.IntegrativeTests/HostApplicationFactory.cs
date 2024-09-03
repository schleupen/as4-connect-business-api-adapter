namespace Schleupen.AS4.BusinessAdapter;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

public class HostApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> 
    where TEntryPoint : class
{
    private readonly Action<IWebHostBuilder> configuration;

    public HostApplicationFactory(Action<IWebHostBuilder> configuration)
    {
        this.configuration = configuration;
    }
    
    public Task RunHostAsync()
    {
        var host = Services.GetRequiredService<IHost>();
        return host.WaitForShutdownAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        configuration(builder.Configure(_ => { }));
}