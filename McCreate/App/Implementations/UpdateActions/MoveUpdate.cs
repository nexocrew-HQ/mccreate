using McCreate.App.Configuration;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Spectre.Console;

namespace McCreate.App.Implementations.UpdateActions;

public class MoveUpdate : IUpdateAction
{
    public string Name { get; set; } = "Move Server";
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();
        
        var newPath = Path.GetFullPath(AnsiConsole.Prompt(
            new TextPrompt<string>(AnsiHelper.QuestionFormat("Where would you like to move your server?"))
                .DefaultValue(Directory.GetCurrentDirectory())
                .DefaultValueStyle(Color.DodgerBlue2)
                .ValidationErrorMessage("[red]That's not a valid path[/]")
                .Validate(path => 
                    Path.IsPathFullyQualified(Path.GetFullPath(path)) 
                    && !configService.Get().Servers.Any(x => x.Path == path) 
                        ? ValidationResult.Success() 
                        : ValidationResult.Error("[red]This is not a vaild path or a server already exists at this path[/]")))
        );
        
        try
        {
            Directory.Move(server.Path, newPath);
        }
        catch (Exception e)
        {
            switch (e)
            {
                case IOException:
                    AnsiHelper.Error("The server could not be moved because: the target directory is already existing, is on a different volume or the source path is equal to the target path.");
                    return;
                default:
                    AnsiHelper.Error($"An unknown error occured while moving your server: {e.Message}");
                    if (e.StackTrace != null)
                        AnsiHelper.Error(e.StackTrace);
                    return;
            }
        }

        var oldPath = server.Path;

        server.Path = newPath;

        await ServerHelper.UpdateServer(server, oldPath, serviceProvider);

        AnsiHelper.Success("Successfully moved server");
        
    }
}