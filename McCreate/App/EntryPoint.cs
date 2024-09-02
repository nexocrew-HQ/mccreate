using McCreate.App.Interfaces;
using mccreate.App.Services;
using McCreate.App.Services;
using Spectre.Console;

namespace McCreate.App;

public class EntryPoint : IEntryPoint
{
    private ServerService ServerService;
    private PluginService PluginService;
    private IServiceProvider ServiceProvider;

    public EntryPoint(ServerService serverService, PluginService pluginService, IServiceProvider serviceProvider)
    {
        ServerService = serverService;
        PluginService = pluginService;
        ServiceProvider = serviceProvider;
    }

    public void Run()
    {
        // TODO:
        // - autodetect version (maybe config file)
        
        AnsiConsole.MarkupLine("[white dim] Welcome to [/][red bold]mccreate[/] [mediumspringgreen]v1.0.0[/]");
        AnsiConsole.MarkupLine("[gray dim] Made by [/][bold blue]nexocrew[/]");
        AnsiConsole.WriteLine();

        var actionsList = PluginService.GetImplementations<IProgramAction>();

        if (actionsList.Length < 1)
            throw new Exception(
                "There are no registered Implementations for IProgramAction, please register one in Program.cs");
        
        
        var selection = new SelectionPrompt<IProgramAction>();

        selection.Title("[yellow]?[/] [white]What would you like to do?[/]")
            .PageSize(5)
            .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
            .AddChoices(actionsList).HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = x => $"[white bold]{x.Name}[/]";

        var action = AnsiConsole.Prompt(selection);

        Task.Run(() => action.Execute(ServiceProvider));

    }
}