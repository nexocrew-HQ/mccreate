using System.Collections.Immutable;
using McCreate.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace McCreate.App.Extensions;

public class ApplicationBuilder
{

    
    public ServiceCollection Services;

    public ApplicationBuilder(IEntryPoint entryPoint)
    {
        Create(entryPoint);
    }
    
    private void Create(IEntryPoint entryPoint)
    {
        Services = new ServiceCollection();

        Services.AddSingleton(entryPoint);

    }

    public Application Build()
    {
        var provider = Services.BuildServiceProvider();
        
        return new Application(provider);
    }
}