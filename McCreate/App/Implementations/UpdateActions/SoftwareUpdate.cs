using McCreate.App.Extensions;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace McCreate.App.Implementations.UpdateActions;

public class SoftwareUpdate : IUpdateAction
{
    public string Name { get; set; } = "Change Software";
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        var implementationService = serviceProvider.GetRequiredService<ImplementationService>();

        var softwareList = implementationService.GetImplementations<IServerSoftware>();
        
        if (softwareList.Length < 1) 
            return;

        var softwareSelection = new SelectionPrompt<IServerSoftware>();
        
        softwareSelection
            .Title(AnsiHelper.QuestionFormat("Which server software would you like to use?"))
            .MoreChoicesText("[grey](Move up and down to reveal more software types)[/]")
            .AddChoices(softwareList)
            .UseDefaultStyles(x => $"[white bold]{x.Name}[/]");
        
        server.Software = AnsiConsole.Prompt(softwareSelection);
        
        AnsiHelper.ConfirmSelection("Selected Software", server.Software.Name);

        await ServerHelper.UpdateServer(server, server.Path, serviceProvider);
        
        AnsiHelper.Success($"Updated server software to {server.Software.Name}");
        
        AnsiHelper.Info("You will need to change the version after changing the software.");

        IUpdateAction versionUpdate = new VersionUpdate();

        await versionUpdate.Update(server, serviceProvider);

    }
}