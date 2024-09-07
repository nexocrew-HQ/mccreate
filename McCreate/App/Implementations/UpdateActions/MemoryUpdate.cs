using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using Spectre.Console;

namespace McCreate.App.Implementations.UpdateActions;

public class MemoryUpdate : IUpdateAction
{
    public string Name { get; set; } = "Change Memory";
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        server.MemoryInMegabytes = AnsiConsole.Prompt(
            new TextPrompt<int>(AnsiHelper.QuestionFormat("How much ram would you like to assign your server (in Megabytes)?"))
                .ValidationErrorMessage("[red]That's not a valid number[/]")
                .DefaultValue(server.MemoryInMegabytes)
                .DefaultValueStyle(Color.DodgerBlue2)
                .Validate(x =>
                {
                    return x switch
                    {
                        <= 0 => ValidationResult.Error("[red]You must assign at least 1 MB of RAM [/]"),
                        _ => ValidationResult.Success(),
                    };
                }));
        
        await ServerHelper.UpdateServer(server, server.Path, serviceProvider);

        await ServerHelper.CreateStartupFiles(server);
        
        AnsiHelper.Success("Successfully updated memory");
    }
}