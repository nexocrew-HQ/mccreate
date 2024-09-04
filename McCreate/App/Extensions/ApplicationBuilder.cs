using McCreate.App.Interfaces;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace McCreate.App.Extensions;

public class ApplicationBuilder<T> where T : class, IEntryPoint
{

    
    public ServiceCollection Services;
    public ImplementationService Implementations;

    public ApplicationBuilder()
    {
        Create();
        Implementations = new ImplementationService();
    }
    
    private void Create()
    {
        Services = new ServiceCollection();
    }

    public Application<T> Build()
    {
        Services.AddSingleton<T>();
        Services.AddSingleton(Implementations);
        
        Services.MakeReadOnly();
        
        var provider = Services.BuildServiceProvider();
        
        return new Application<T>(provider);
    }
}