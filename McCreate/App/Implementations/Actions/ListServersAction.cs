using McCreate.App.Configuration;
using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Spectre.Console;

namespace McCreate.App.Implementations.Actions;

public class ListServersAction : IProgramAction
{
    public string Name { get; set; } = "List Created Servers";

    public string Description { get; set; } = "";

    public async Task Execute(IServiceProvider serviceProvider)
    {
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();

        var configServers = configService.Get().Servers;

        var servers = await ServerHelper.ConvertConfigServersToServers(configServers, serviceProvider);

        
        var table = new Table
        {
            Border = TableBorder.HeavyHead
        };

        table.AddColumn(new TableColumn("[yellow]Software[/]").Centered());
        table.AddColumn(new TableColumn("[green]Version[/]").Centered());
        table.AddColumn(new TableColumn("[blue]Path[/]").Centered());
        table.AddColumn(new TableColumn("[deeppink3]Memory[/]").Centered());
        
        foreach (var server in servers)
        {
            table.AddRow(server.Software.Name, server.Version.Name, server.Path, server.MemoryInMegabytes + "MB");
        }

        
        AnsiConsole.Write(table);
        
    }
}