using Microsoft.Extensions.DependencyInjection;

namespace mccreate.App.Extensions;

public class Application
{
    public ServiceProvider Services;

    public Application(ServiceProvider services)
    {
        Services = services;
    }
    
}