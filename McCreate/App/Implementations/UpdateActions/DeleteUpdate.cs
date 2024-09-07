using McCreate.App.Configuration;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Spectre.Console;

namespace McCreate.App.Implementations.UpdateActions;

public class DeleteUpdate : IUpdateAction
{
    public string Name { get; set; } = "Delete Server";
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        var confim = AnsiConsole.Confirm(AnsiHelper.QuestionFormat("Do you really want to delete the server? This will also delete SERVER FILES"), defaultValue: false);

        if (!confim)
            return;
        try
        {
            Directory.Delete(server.Path, true);
        }
        catch (DirectoryNotFoundException e)
        {
            // ignored
        }
        
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();

        var itemToDelete = configService.Get().Servers.First(x => x.Path == server.Path);
        
        configService.Get().Servers.Remove(itemToDelete);
        
        configService.Save();
    }
}