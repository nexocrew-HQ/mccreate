using System.Collections.Immutable;
using McCreate.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace McCreate.App.Extensions;

public class ApplicationBuilder
{

    
    public ServiceCollection Services;

    private IEntryPoint EntryPoint;

    public ApplicationBuilder(IEntryPoint entryPoint)
    {
        EntryPoint = entryPoint;
        Create();
    }
    
    private void Create()
    {
        Services = new ServiceCollection();
    }

    public Application Build()
    {
        Services.AddSingleton(EntryPoint);
        
        var provider = Services.BuildServiceProvider();
        
        return new Application(provider);
    }
}