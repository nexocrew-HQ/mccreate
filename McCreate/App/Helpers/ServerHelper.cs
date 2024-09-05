using McCreate.App.Configuration;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.Services;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Helpers;

public class ServerHelper
{
    public static async Task CreateStartupFiles(string path, int ram, string additionalFlags)
    {
        var startupCommand = $"java -Xms{ram}M -Xmx{ram}M {additionalFlags} -jar server.jar nogui";

        var serverPath = Path.GetFullPath(path);
        
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

    public static async Task SaveServerToConfig(IServiceProvider serviceProvider, Server server)
    {
        var configService = serviceProvider.GetRequiredService<ConfigService<ConfigModel>>();

        configService.Get().Servers.Add(server);
        
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
    
}