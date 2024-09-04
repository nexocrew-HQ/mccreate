using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;
using McCreate.App.Models.ServerSoftwareModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Spectre.Console;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Implementations.ServerSoftwares;

public class Purpur : IServerSoftware
{
    public string Name { get; set; } = "Purpur";

    public string UniqueId { get; set; } = "minecraftpurpur";

    public async Task<List<VersionGroup>> GetAvailableVersions(IServiceProvider serviceProvider)
    {
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();

        string response;
        
        try
        {
            response = await httpClient.GetStringAsync("https://api.purpurmc.org/v2/purpur");
        }
        catch (Exception e)
        {
            Console.WriteLine("An unhandled error occured while fetching the purpur versions");
            Console.WriteLine(e.Message);
            throw;
        }
        
        var versions = JsonConvert.DeserializeObject<PurpurApiParser.VersionApi>(response)!.Versions;

        var versionGroups = await VersionHelper.GetVersionGroupListFromListOfVersions(versions);
        
        return versionGroups;
    }

    public async Task DownloadVersion(Version version, string path, IServiceProvider serviceProvider)
    {
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();

        string response;
        
        try
        {
            response = await httpClient.GetStringAsync($"https://api.purpurmc.org/v2/purpur/{version.Name}");
        }
        catch (Exception e)
        {
            Console.WriteLine("An unhandled error occured while fetching the purpur versions");
            Console.WriteLine(e.Message);
            throw;
        }

        var buildNumbers = JsonConvert.DeserializeObject<PurpurApiParser.DownloadApi.BuildsApi>(response);

        var allBuildNumbers = new List<int>();

        var latestBuildNumber = int.Parse(buildNumbers!.Builds.LatestBuild.Trim());


        allBuildNumbers.Add(latestBuildNumber);

        allBuildNumbers
            .AddRange(
                from buildNumber in buildNumbers.Builds.AllBuils 
                where int.Parse(buildNumber.Trim()) != latestBuildNumber 
                select int.Parse(buildNumber.Trim()));

        allBuildNumbers.Sort((x, y) => y.CompareTo(x));

        int stableBuildNumber = 0;

        foreach (var buildNumber in allBuildNumbers)
        {
            string buildResponse;
        
            try
            {
                buildResponse = await httpClient.GetStringAsync($"https://api.purpurmc.org/v2/purpur/{version.Name}/{buildNumber}");
            }
            catch (Exception e)
            {
                Console.WriteLine("An unhandled error occured while fetching the purpur versions");
                Console.WriteLine(e.Message);
                throw;
            }

            var isStable = JsonConvert.DeserializeObject<PurpurApiParser.DownloadApi.BuildSuccess>(buildResponse)!.Result == "SUCCESS";

            if (!isStable) continue;
            
            stableBuildNumber = buildNumber;
        }

        if (stableBuildNumber == 0)
        {
            throw new Exception($"Could not find any stable build for {version.Name}");
        }

        var downloadUrl = $"https://api.purpurmc.org/v2/purpur/{version.Name}/{stableBuildNumber}/download";
        
        // ensure the directory exists before we change into it
        Directory.CreateDirectory(path);
        
        // set the current working directory to the servers path
        Directory.SetCurrentDirectory(path);
        
        // copy the file from the url to the pa
        
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
        return "";
    }
}
