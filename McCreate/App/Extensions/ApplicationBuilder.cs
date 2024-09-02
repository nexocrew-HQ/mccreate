using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace mccreate.App.Extensions;

public class ApplicationBuilder
{

    
    public ServiceCollection Services;

    public ApplicationBuilder()
    {
        Create();
    }
    
    private void Create()
    {
        Services = new ServiceCollection();
    }

    public Application Build()
    {
        var provider = Services.BuildServiceProvider();

        return new Application(provider);
    }
}