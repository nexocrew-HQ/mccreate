using System.Collections.Immutable;
using McCreate.App.Interfaces;
using mccreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace McCreate.App.Extensions;

public class ApplicationBuilder<T> where T : class, IEntryPoint
{

    
    public ServiceCollection Services;
    public PluginService Plugins;

    public ApplicationBuilder()
    {
        Create();
        Plugins = new PluginService();
    }
    
    private void Create()
    {
        Services = new ServiceCollection();
    }

    public Application<T> Build()
    {
        Services.AddSingleton<T>();
        Services.AddSingleton(Plugins);
        
        Services.MakeReadOnly();
        
        var provider = Services.BuildServiceProvider();
        
        return new Application<T>(provider);
    }
}