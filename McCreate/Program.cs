using McCreate.App;
using McCreate.App.Configuration;
using McCreate.App.Extensions;
using McCreate.App.Helpers;
using McCreate.App.Implementations.Actions;
using McCreate.App.Implementations.ServerSoftwares;
using McCreate.App.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoonCore.Services;

namespace McCreate;

class Program
{
    static void Main(string[] args)
    {
        
        ApplicationBuilder<EntryPoint> applicationBuilder = new();
        
        // Add Services here
        // applicationBuilder.Services.AddSingleton<YourService>();
        // just use singletons, because other service types wouldn't really make sense in this project
        
        applicationBuilder.Services.AddSingleton<HttpClient>();

        ConfigHelper configHelper = new();

        string configPath = configHelper.Perform();

        applicationBuilder.Services.AddSingleton(
            new ConfigService<ConfigModel>(configPath));
        
        // Add Plugins here
        // applicationBuilder.Implementations.RegisterImplementation<IInterface>(new Implementation());
        
        applicationBuilder.Implementations.RegisterImplementation<IProgramAction, CreateServerAction>();
        applicationBuilder.Implementations.RegisterImplementation<IProgramAction, ListServersAction>();
        
        applicationBuilder.Implementations.RegisterImplementation<IServerSoftware, Paper>();
        applicationBuilder.Implementations.RegisterImplementation<IServerSoftware, Purpur>();

        
        // Build the application
        Application<EntryPoint> application = applicationBuilder.Build();

        // Start the application
        application.Start();

    }
}