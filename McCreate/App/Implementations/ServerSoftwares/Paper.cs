using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Models.Paper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Version = McCreate.App.Models.Version;

namespace mccreate.App.Implementations.ServerSoftwares;

public class Paper : IServerSoftware
{
    public string Name { get; set; } = "Paper";

    public async Task<List<VersionGroup>> GetAvailableVersions(IServiceProvider serviceProvider)
    {
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();

        string response;
        
        try
        {
            response = await httpClient.GetStringAsync("https://api.papermc.io/v2/projects/paper/");
        }
        catch (Exception e)
        {
            Console.WriteLine("An unhandled error occured while fetching the paper versions");
            Console.WriteLine(e.Message);
            throw;
        }
        
        
        var data = JsonConvert.DeserializeObject<PaperApiParser.VersionApi>(response);

        var versionGroupList = new List<VersionGroup>();
        
        foreach (var versionGroup in data.VersionGroups)
        {
            var newVersionGroup = new VersionGroup()
            {
                PrimaryVersion = versionGroup
            };

            foreach (var version in data.Versions.Where(version => version.StartsWith(versionGroup)))
            {
                newVersionGroup.SubVersions.Add(new Version()
                {
                    Name = version
                });
            }
            
            versionGroupList.Add(newVersionGroup);
            
        }
        
        return versionGroupList;
    }

    public async Task DownloadVersion(Version version, string path, IServiceProvider serviceProvider)
    {
        
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        
        string response;
        
        try
        {
            response = await httpClient.GetStringAsync($"https://api.papermc.io/v2/projects/paper/versions/{version.Name}/builds");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An unhandled error occured while downloading paper version {version.Name}");
            Console.WriteLine(e.Message);
            throw;
        }

        var stableBuilds = JsonConvert
            .DeserializeObject<PaperApiParser.DownloadApi>(response)!
            .Builds
            .Where(x => x.Channel == "default")
            .OrderByDescending(x => x.BuildNumber)
            .ToList();


        PaperApiParser.DownloadApi.Build? latestStableBuild = stableBuilds.FirstOrDefault();

        if (latestStableBuild == null)
        {
            throw new Exception($"Could not find any stable builds for paper {version.Name}");
        }

        var downloadUrl =
            $"https://api.papermc.io/v2/projects/paper/versions/{version.Name}/builds/{latestStableBuild.BuildNumber}/downloads/paper-{version.Name}-{latestStableBuild.BuildNumber}.jar";

        // ensure the directory exists before we change into it
        Directory.CreateDirectory(path);
        
        // set the current working directory to the servers path
        Directory.SetCurrentDirectory(path);
        
        // copy the file from the url to the path
        var stream = await httpClient.GetStreamAsync(downloadUrl);
        var fs = new FileStream("server.jar", FileMode.OpenOrCreate);
        await stream.CopyToAsync(fs);
    }

    public async Task DoFinalSteps(string path, IServiceProvider serviceProvider)
    {
        Directory.SetCurrentDirectory(path);
        
        var eula = AnsiConsole.Confirm(AnsiHelper.QuestionFormat("Do you agree to Minecraft's EULA?"), defaultValue: false);

        await using var sw = new StreamWriter("eula.txt", false);
        
        await sw.WriteLineAsync("eula=" + eula.ToString().ToLower());
        sw.Close();
    }

    public async Task<string> GetAdditionalFlags(IServiceProvider serviceProvider)
    {
        var flags = AnsiConsole.Confirm(AnsiHelper.QuestionFormat("Do you want to use aikars flags for better performance? (recommended)"), defaultValue: true);

        return flags ? "-XX:+AlwaysPreTouch -XX:+DisableExplicitGC -XX:+ParallelRefProcEnabled -XX:+PerfDisableSharedMem -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1HeapRegionSize=8M -XX:G1HeapWastePercent=5 -XX:G1MaxNewSizePercent=40 -XX:G1MixedGCCountTarget=4 -XX:G1MixedGCLiveThresholdPercent=90 -XX:G1NewSizePercent=30 -XX:G1RSetUpdatingPauseTimePercent=5 -XX:G1ReservePercent=20 -XX:InitiatingHeapOccupancyPercent=15 -XX:MaxGCPauseMillis=200 -XX:MaxTenuringThreshold=1 -XX:SurvivorRatio=32 -Dusing.aikars.flags=https://mcflags.emc.gs -Daikars.new.flags=true" : "";
    }
}
