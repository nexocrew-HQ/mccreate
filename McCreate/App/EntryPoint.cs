using McCreate.App.Interfaces;
using McCreate.App.Services;
using Spectre.Console;

namespace McCreate.App;

public class EntryPoint : IEntryPoint
{
    private ServerService ServerService;

    public EntryPoint(ServerService serverService)
    {
        ServerService = serverService;
    }

    public void Run()
    {
        // TODO:
        // - autodetect version (maybe config file)
        
        AnsiConsole.MarkupLine("[white dim] Welcome to [/][red bold]mccreate[/] [mediumspringgreen]v1.0.0[/]");
        AnsiConsole.MarkupLine("[gray dim] Made by [/][bold blue]nexocrew[/]");
        AnsiConsole.WriteLine();
        
        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]?[/] [white]What would you like to do?[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
                .AddChoices([
                    "Create a new server",
                    "List all created servers",
                    "Update a server",
                    "Delete a server"
                ]));

        switch (action)
        {
            case "Create a new server":
                ServerService.Create();
                break;
            case "List all created servers":
                ServerService.List();
                break;
            case "Update a server":
                ServerService.Update();
                break;
            case "Delete a server":
                ServerService.Delete();
                break;
        }
        
    }
}