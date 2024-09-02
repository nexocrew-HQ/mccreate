using McCreate.App;
using McCreate.App.Extensions;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace McCreate;

class Program
{
    static void Main(string[] args)
    {
        
        ApplicationBuilder applicationBuilder = new(new EntryPoint());
        
        // Add Services here
        // applicationBuilder.Services.AddSingleton<YourService>();
        
        applicationBuilder.Services.AddSingleton<TestService>();
        applicationBuilder.Services.AddSingleton<MccreateService>();

        // Build the application
        Application application = applicationBuilder.Build();

        // Start the application
        application.Start();

    }
}