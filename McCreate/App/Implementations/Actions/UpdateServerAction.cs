using McCreate.App.Configuration;
using McCreate.App.Extensions;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Spectre.Console;

namespace McCreate.App.Implementations.Actions;

public class UpdateServerAction : IProgramAction
{
    public string Name { get; set; } = "Update a Server";
    
    public string Description { get; set; } = "Update a servers, software, version, memory and flags";
    public async Task Execute(IServiceProvider serviceProvider)
    {
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();

        var servers = await ServerHelper.ConvertConfigServersToServers(configService.Get().Servers, serviceProvider);
        
        var serverSelection = new SelectionPrompt<Server>();

        serverSelection
            .Title(AnsiHelper.QuestionFormat("Which server would you like to update?"))
            .MoreChoicesText("[grey](Move up and down to reveal more servers)[/]")
            .AddChoices(servers)
            .UseDefaultStyles(x => $"[white bold]{x.Software.Name} {x.Version.Name} {x.Path}[/]");

        var server = AnsiConsole.Prompt(serverSelection);

        var implementationService = serviceProvider.GetRequiredService<ImplementationService>();

        var updateImplementations = implementationService.GetImplementations<IUpdateAction>();
        
        var actionSelection = new SelectionPrompt<IUpdateAction>();
        
        actionSelection
            .Title(AnsiHelper.QuestionFormat("What would you like to update?"))
            .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
            .AddChoices(updateImplementations)
            .UseDefaultStyles(x => $"[white bold]{x.Name}[/]");
        
        var action = AnsiConsole.Prompt(actionSelection);

        await action.Update(server, serviceProvider);
    }
}