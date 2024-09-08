using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Services;
using Spectre.Console;

namespace McCreate.App;

public class EntryPoint : IEntryPoint
{
    private ImplementationService ImplementationService;
    private IServiceProvider ServiceProvider;

    public EntryPoint(ImplementationService implementationService, IServiceProvider serviceProvider)
    {
        ImplementationService = implementationService;
        ServiceProvider = serviceProvider;
    }

    public void Run()
    {
        // TODO:
        // - autodetect version (maybe config file)
        
        AnsiHelper.Title("blueviolet bold");
        
        AnsiConsole.MarkupLine("[grey] Created by [/][bold blue]Moritz[/][grey], brought to you by[/] [bold blue]nexocrew[/]");
        AnsiConsole.MarkupLine("[mediumspringgreen dim]  v1.0.0[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim white]You need help with mccreate? Then Navigate to https://nexocrew.link/s/mccreatedocs[/]");
        AnsiConsole.WriteLine();

        var actionsList = ImplementationService.GetImplementations<IProgramAction>();

        if (actionsList.Length < 1)
            throw new Exception(
                "There are no registered Implementations for IProgramAction, please register one in Program.cs");
        
        
        var selection = new SelectionPrompt<IProgramAction>();

        selection
            .Title(AnsiHelper.QuestionFormat("What would you like to do?"))
            .PageSize(5)
            .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
            .AddChoices(actionsList).HighlightStyle(new Style().Foreground(Color.DodgerBlue2))
            .Converter = x => $"[white bold]{x.Name}[/]";

        var action = AnsiConsole.Prompt(selection);

        AnsiHelper.ConfirmSelection("Selected action", action.Name);
        
        AnsiHelper.ConfirmSelection("Description", action.Description);
        
        action.Execute(ServiceProvider).GetAwaiter().GetResult();
    }
}