namespace McCreate.App.Helpers;

public class StartupCommandHelper
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
}