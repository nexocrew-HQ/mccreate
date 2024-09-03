using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using mccreate.App.Services;
using McCreate.App.Services;
using Spectre.Console;

namespace McCreate.App;

public class EntryPoint : IEntryPoint
{
    private ServerService ServerService;
    private ImplementationService ImplementationService;
    private IServiceProvider ServiceProvider;

    public EntryPoint(ServerService serverService, ImplementationService implementationService, IServiceProvider serviceProvider)
    {
        ServerService = serverService;
        ImplementationService = implementationService;
        ServiceProvider = serviceProvider;
    }

    public void Run()
    {
        // TODO:
        // - autodetect version (maybe config file)
        
        ConsoleHelper.Title("blueviolet bold");
        
        AnsiConsole.MarkupLine("[grey] Created by [/][bold blue]Moritz[/][grey], brought to you by[/] [bold blue]nexocrew[/]");
        AnsiConsole.MarkupLine("[mediumspringgreen dim]  v1.0.0[/]");
        AnsiConsole.WriteLine();

        var actionsList = ImplementationService.GetImplementations<IProgramAction>();

        if (actionsList.Length < 1)
            throw new Exception(
                "There are no registered Implementations for IProgramAction, please register one in Program.cs");
        
        
        var selection = new SelectionPrompt<IProgramAction>();

        selection
            .Title(ConsoleHelper.QuestionFormat("What would you like to do?"))
            .PageSize(5)
            .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
            .AddChoices(actionsList).HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = x => $"[white bold]{x.Name}[/]";

        var action = AnsiConsole.Prompt(selection);

        ConsoleHelper.ConfirmSelection("Selected action", action.Name);
        
        action.Execute(ServiceProvider).GetAwaiter().GetResult();
    }
}