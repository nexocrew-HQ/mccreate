using mccreate.App.Extensions;
using mccreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace mccreate;

class Program
{
    static void Main(string[] args)
    {
        ApplicationBuilder applicationBuilder = new();
        
        // Add Services here
        // applicationBuilder.Services.AddSingleton<YourService>();
        
        applicationBuilder.Services.AddSingleton<MccreateService>();

        // Build the application
        Application application = applicationBuilder.Build();

        var service = application.Services.GetRequiredService<MccreateService>();

    }
}