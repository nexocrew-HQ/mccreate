using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using mccreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Version = McCreate.App.Models.Version;

namespace mccreate.App.Implementations.Actions;

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
            .PageSize(5)
            .MoreChoicesText("[grey](Move up and down to reveal more software types)[/]")
            .AddChoices(serverSoftwareList)
            .HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = x => $"[white bold]{x.Name}[/]";

        var software = AnsiConsole.Prompt(softwareSelection);
        
        AnsiConsole.MarkupLine($"[blue]-[/] [white]Selected Software: [/][yellow]{software.Name}[/][white].[/]");

        var versionGroupList = await software
            .GetAvailableVersions(serviceProvider);
            
        
        var versionSelection = new SelectionPrompt<VersionGroup>();

        versionSelection
            .Title(ConsoleHelper.QuestionFormat("Which server version would you like to use?"))
            .PageSize(8)
            .MoreChoicesText("[grey](Move up and down to reveal more versions)[/]")
            .AddChoices(versionGroupList)
            .HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = x => $"[white bold]{x.PrimaryVersion}[/]";

        var version = AnsiConsole.Prompt(versionSelection);

        ConsoleHelper.ConfirmSelection("Selected version", version.PrimaryVersion);
        
        var subVersionSelection = new SelectionPrompt<Version>();
        
        subVersionSelection
            .Title(ConsoleHelper.QuestionFormat("Which server subversion would you like to use?"))
            .PageSize(8)
            .MoreChoicesText("[grey](Move up and down to reveal more versions)[/]")
            .AddChoices(version.SubVersions)
            .HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = x => $"[white bold]{x.Name}[/]";
        
        
        var subVersion = AnsiConsole.Prompt(subVersionSelection);
        
        ConsoleHelper.ConfirmSelection("Selected subversion", subVersion.Name);
        
        var serverPath = AnsiConsole.Ask(ConsoleHelper.QuestionFormat("Where would you like to save your server?"), Directory.GetCurrentDirectory());
        
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Line)
            .StartAsync("Downloading Version...", async ctx => {
                await software.DownloadVersion(subVersion, serverPath, serviceProvider);
            });
        
        ConsoleHelper.ConfirmSelection("Successfully downloaded", software.Name + " " + subVersion.Name);

        await software.DoFinalSteps(serverPath, serviceProvider);
        
        var ram = AnsiConsole.Prompt(
            new TextPrompt<int>(ConsoleHelper.QuestionFormat("How much ram would you like to assign your server (in Megabytes)?"))
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

        var startupCommand = $"java -Xms{ram}M -Xmx{ram}M {additionalFlags} -jar server.jar nogui";


        // ensure the directory exists before we change into it
        Directory.CreateDirectory(serverPath);
        
        // set the current working directory to the servers path
        Directory.SetCurrentDirectory(serverPath);
        
        await using var windowsStreamWriter = new StreamWriter("start.bat", false);
        
        await windowsStreamWriter.WriteLineAsync($"@echo off \n{startupCommand} \n" +
                                                 $"pause");
        windowsStreamWriter.Close();
        
        await using var linuxMacOsStreamWriter = new StreamWriter($"start.sh", false);
        
        await linuxMacOsStreamWriter.WriteLineAsync($"#!/usr/bin/env\n{startupCommand}");
        linuxMacOsStreamWriter.Close();

    }
}
