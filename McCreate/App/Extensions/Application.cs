using McCreate.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace McCreate.App.Extensions;

public class Application
{
    public ServiceProvider Services;

    public Application(ServiceProvider services)
    {
        Services = services;
    }

    public void Start()
    {
        Services.GetRequiredService<IEntryPoint>().Run();
    }
    
}