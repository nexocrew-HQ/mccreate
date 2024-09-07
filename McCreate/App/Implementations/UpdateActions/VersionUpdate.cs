using McCreate.App.Extensions;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using Spectre.Console;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Implementations.UpdateActions;

public class VersionUpdate : IUpdateAction
{
    public string Name { get; set; } = "Change Version";
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        var versionGroupList = await server.Software.GetAvailableVersions(serviceProvider);
        
        var versionGroupSelection = new SelectionPrompt<VersionGroup>();

        versionGroupSelection
            .Title(AnsiHelper.QuestionFormat("Which server version would you like to use?"))
            .MoreChoicesText("[grey](Move up and down to reveal more versions)[/]")
            .AddChoices(versionGroupList)
            .UseDefaultStyles(x => $"[white bold]{x.PrimaryVersion}[/]");

        var versionGroup = AnsiConsole.Prompt(versionGroupSelection);
        
        AnsiHelper.ConfirmSelection("Selected version", versionGroup.PrimaryVersion);
        
        var subVersionSelection = new SelectionPrompt<Version>();

        subVersionSelection
            .Title(AnsiHelper.QuestionFormat("Which server subversion would you like to use?"))
            .AddChoices(versionGroup.SubVersions)
            .UseDefaultStyles(x => $"[white bold]{x.Name}[/]")
            .MoreChoicesText("[grey](Move up and down to reveal more versions)[/]");
        
        server.Version = AnsiConsole.Prompt(subVersionSelection);
        
        AnsiHelper.ConfirmSelection("Selected subversion", server.Version.Name);
        
        await ServerHelper.UpdateServer(server, server.Path, serviceProvider);
        
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Line)
            .StartAsync($"Downloading {server.Software.Name} Version {server.Version.Name}...", async ctx => {
                await server.Software.DownloadVersion(server.Version, server.Path, serviceProvider);
            });
        
        AnsiHelper.Success("Successfully updated version");
    }
}