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

    public async Task Execute(IServiceProvider serviceProvider)
    {
        var implementationService = serviceProvider.GetRequiredService<ImplementationService>();

        var serverSoftwareList = implementationService.GetImplementations<IServerSoftware>();

        if (serverSoftwareList.Length < 1)
        {
            throw new Exception(
                    "There are no registered Implementations for IServerSoftware, please register one in Program.cs");
        }
        
        var softwareSelection = new SelectionPrompt<IServerSoftware>();

        softwareSelection
            .Title("[yellow]?[/] [white]Which server software would you like to use?[/]")
            .MoreChoicesText("[grey](Move up and down to reveal more software types)[/]")
            .AddChoices(serverSoftwareList)
            .UseDefaultStyles(x => $"[white bold]{x.Name}");

        var software = AnsiConsole.Prompt(softwareSelection);
        
        AnsiHelper.ConfirmSelection("Selected software", software.Name);
        
        var versionGroupList = await software
            .GetAvailableVersions(serviceProvider);
            
        
        var versionSelection = new SelectionPrompt<VersionGroup>();

        versionSelection
            .Title(AnsiHelper.QuestionFormat("Which server version would you like to use?"))

            .MoreChoicesText("[grey](Move up and down to reveal more versions)[/]")
            .AddChoices(versionGroupList)
            .UseDefaultStyles(x => $"[white bold]{x.PrimaryVersion}");

        var version = AnsiConsole.Prompt(versionSelection);

        AnsiHelper.ConfirmSelection("Selected version", version.PrimaryVersion);
        
        var subVersionSelection = new SelectionPrompt<Version>();

        subVersionSelection
            .Title(AnsiHelper.QuestionFormat("Which server subversion would you like to use?"))
            .AddChoices(version.SubVersions)
            .UseDefaultStyles(x => $"[white bold]{x.Name}")
            .MoreChoicesText("[grey](Move up and down to reveal more versions)[/]");
        
        var subVersion = AnsiConsole.Prompt(subVersionSelection);
        
        AnsiHelper.ConfirmSelection("Selected subversion", subVersion.Name);
        
        var serverPath = Path.GetFullPath(AnsiConsole.Prompt(
            new TextPrompt<string>(AnsiHelper.QuestionFormat("Where would you like to save your server?"))
                .DefaultValue(Directory.GetCurrentDirectory())
                .DefaultValueStyle(Color.DodgerBlue2)
                .ValidationErrorMessage("[red]That's not a valid path[/]")
                .Validate(path => Path.IsPathFullyQualified(Path.GetFullPath(path)) ? ValidationResult.Success() : ValidationResult.Error("[red]This is not a vaild path[/]")))
        );
        
        AnsiHelper.ConfirmSelection("Saving server to", serverPath);
        
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Line)
            .StartAsync($"Downloading {software.Name} Version {subVersion.Name}...", async ctx => {
                await software.DownloadVersion(subVersion, serverPath, serviceProvider);
            });
        
        AnsiHelper.ConfirmSelection("Successfully downloaded", software.Name + " " + subVersion.Name);

        await software.DoFinalSteps(serverPath, serviceProvider);
        
        var ram = AnsiConsole.Prompt(
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

        var additionalFlags = await software.GetAdditionalFlags(serviceProvider);

        await StartupCommandHelper.CreateStartupFiles(serverPath, ram, additionalFlags);

    }
}
