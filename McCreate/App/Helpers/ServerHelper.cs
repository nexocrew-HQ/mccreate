using McCreate.App.Configuration;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Services;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Spectre.Console;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Helpers;

public class ServerHelper
{
    public static async Task CreateStartupFiles(Server server)
    {
        var startupCommand = $"java -Xms{server.MemoryInMegabytes}M -Xmx{server.MemoryInMegabytes}M {server.AdditionalFlags} -jar server.jar nogui";

        var serverPath = Path.GetFullPath(server.Path);
        
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

    public static async Task<List<Server>> ConvertConfigServersToServers(IEnumerable<ConfigServer> configServers, IServiceProvider serviceProvider)
    {
        var implementationService = serviceProvider.GetRequiredService<ImplementationService>();

        var servers = new List<Server>();
        
        var softwareImplementations = implementationService.GetImplementations<IServerSoftware>().ToList();
        
        foreach (var configServer in configServers)
        {
            var serverSoftware = softwareImplementations.FirstOrDefault(x => x.UniqueId == configServer.SoftwareId);

            if (serverSoftware == null)
            {
                AnsiConsole.MarkupLine($"[red bold]Could not get the software of server \"{configServer.Path}\"[/]");
                continue;
            }

            var newServer = new Server()
            {
                Software = serverSoftware,
                Version = configServer.Version,
                AdditionalFlags = configServer.AdditionalFlags,
                MemoryInMegabytes = configServer.MemoryInMegabytes,
                Path = configServer.Path
            };
            
            newServer.Software = serverSoftware;

            servers.Add(newServer);
        }

        return servers;
    }

    public static async Task<ConfigServer> ConvertServerToConfigServer(Server server)
    {
        var configServer = new ConfigServer()
        {
            SoftwareId = server.Software.UniqueId,
            AdditionalFlags = server.AdditionalFlags,
            MemoryInMegabytes = server.MemoryInMegabytes,
            Path = server.Path,
            Version = server.Version
        };

        return configServer;
    }

    public static async Task SaveServerToConfig(IServiceProvider serviceProvider, Server server)
    {
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();

        var configServer = await ConvertServerToConfigServer(server);
        
        configService.Get().Servers.Add(configServer);
        
        configService.Save();
    }
    
    public static async Task<List<VersionGroup>> GetVersionGroupListFromListOfVersions(IEnumerable<string> versions)
    {
        var versionGroups = new List<VersionGroup>();
        
        foreach (var version in versions)
        {
            var parts = version.Split(".");

            if (parts.Length == 2)
            {
                var newGroup = new VersionGroup()
                {
                    PrimaryVersion = version
                };
                
                newGroup.SubVersions.Add(new Version()
                {
                    Name = version
                });
                
                versionGroups.Add(newGroup);
            }
            else if (parts.Length > 2)
            {
                var parentVersion = $"{parts[0]}.{parts[1]}";

                var parentGroup = versionGroups.FirstOrDefault(x => x.PrimaryVersion == parentVersion);

                if (parentGroup == null)
                {
                    var newGroup = new VersionGroup()
                    {
                        PrimaryVersion = parentVersion
                    };
                    
                    newGroup.SubVersions.Add(new Version()
                    {
                        Name = version
                    });
                    
                    versionGroups.Add(newGroup);
                    
                    continue;
                }
                
                parentGroup.SubVersions.Add(new Version()
                {
                    Name = version
                });
            }
        }

        return versionGroups;
    }

    public static async Task UpdateServer(Server server, string originalServerPath, IServiceProvider serviceProvider)
    {
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();

        var newConfigServer = await ConvertServerToConfigServer(server);

        if (configService.Get().Servers.FirstOrDefault(x => x.Path == originalServerPath) == null)
            return;

        var servers = configService.Get().Servers;

        var index = servers.FindIndex(x => x.Path == originalServerPath);

        servers[index] = newConfigServer;

        configService.Get().Servers = servers;
        configService.Save();
    }
    
}