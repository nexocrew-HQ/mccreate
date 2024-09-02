﻿using McCreate.App;
using McCreate.App.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace McCreate;

class Program
{
    static void Main(string[] args)
    {
        
        ApplicationBuilder<EntryPoint> applicationBuilder = new();
        
        // Add Services here
        // applicationBuilder.Services.AddSingleton<YourService>();
        // just use singletons, because other services wouldn't really make sense in this project

        // Build the application
        Application<EntryPoint> application = applicationBuilder.Build();

        // Start the application
        application.Start();

    }
}