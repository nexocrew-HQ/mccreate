using System.Reflection.Metadata;
using McCreate.App;
using McCreate.App.Extensions;
using McCreate.App.Implementations;
using McCreate.App.Interfaces;
using mccreate.App.Services;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace McCreate;

class Program
{
    static void Main(string[] args)
    {
        
        ApplicationBuilder<EntryPoint> applicationBuilder = new();
        
        // Add Services here
        // applicationBuilder.Services.AddSingleton<YourService>();
        // just use singletons, because other service types wouldn't really make sense in this project
        applicationBuilder.Services.AddSingleton<ServerService>();
        

        // Add Plugins here
        // applicationBuilder.Plugins.RegisterImplementation<IInterface>(new Implementation());
        
        applicationBuilder.Plugins.RegisterImplementation<IProgramAction>(new CreateServerAction());

        
        // Build the application
        Application<EntryPoint> application = applicationBuilder.Build();

        // Start the application
        application.Start();

    }
}