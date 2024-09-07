using McCreate.App.Configuration;
using McCreate.App.Extensions;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Spectre.Console;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Implementations.Actions;

public class CreateServerAction : IProgramAction
{
    public string Name { get; set; } = "Create new Server";

    public string Description { get; set; } = "Create a new server with parameters such as: software, version, memory";

    public async Task Execute(IServiceProvider serviceProvider)
    {
        var implementationService = serviceProvider.GetRequiredService<ImplementationService>();

        var serverSoftwareList = implementationService.GetImplementations<IServerSoftware>();

        if (serverSoftwareList.Length < 1)
        {
            throw new Exception(
                    "There are no registered Implementations for IServerSoftware, please register one in Program.cs");
        }

        var server = new Server();
        
        var softwareSelection = new SelectionPrompt<IServerSoftware>();

        softwareSelection
            .Title(AnsiHelper.QuestionFormat("Which server software would you like to use?"))
            .MoreChoicesText("[grey](Move up and down to reveal more software types)[/]")
            .AddChoices(serverSoftwareList)
            .UseDefaultStyles(x => $"[white bold]{x.Name}[/]");

        server.Software = AnsiConsole.Prompt(softwareSelection);
        
        AnsiHelper.ConfirmSelection("Selected software", server.Software.Name);
        
        var versionGroupList = await server.Software
            .GetAvailableVersions(serviceProvider);
        
        
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

        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();
        
        server.Path = Path.GetFullPath(AnsiConsole.Prompt(
            new TextPrompt<string>(AnsiHelper.QuestionFormat("Where would you like to save your server?"))
                .DefaultValue(Directory.GetCurrentDirectory())
                .DefaultValueStyle(Color.DodgerBlue2)
                .ValidationErrorMessage("[red]That's not a valid path[/]")
                .Validate(path => 
                    
                    Path.IsPathFullyQualified(Path.GetFullPath(path)) && !configService.Get().Servers.Any(x => x.Path == path) ? ValidationResult.Success() : ValidationResult.Error("[red]This is not a vaild path or a server already exists at this path[/]")))
        );
        
        AnsiHelper.ConfirmSelection("Saving server to", server.Path);
        
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Line)
            .StartAsync($"Downloading {server.Software.Name} Version {server.Version.Name}...", async ctx => {
                await server.Software.DownloadVersion(server.Version, server.Path, serviceProvider);
            });
        
        AnsiHelper.Success("Successfully downloaded " + server.Software.Name + " " + server.Version.Name);

        await server.Software.DoFinalSteps(server.Path, serviceProvider);
        
        server.MemoryInMegabytes = AnsiConsole.Prompt(
            new TextPrompt<int>(AnsiHelper.QuestionFormat("How much ram would you like to assign your server (in Megabytes)?"))
                .ValidationErrorMessage("[red]That's not a valid number[/]")
                .Validate(x =>
                {
                    return x switch
                    {
                        <= 0 => ValidationResult.Error("[red]You must assign at least 1 MB of RAM [/]"),
                        _ => ValidationResult.Success(),
                    };
                }));

        server.AdditionalFlags = await server.Software.GetAdditionalFlags(serviceProvider);

        await ServerHelper.CreateStartupFiles(server);

        await ServerHelper.SaveServerToConfig(serviceProvider, server);

        AnsiHelper.Success("Successfully created your server, have fun!");
        
    }
}
