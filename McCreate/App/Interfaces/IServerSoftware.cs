using McCreate.App.Models;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Interfaces;

public interface IServerSoftware
{
    string Name { get; set; }
    
    string UniqueId { get; set; }

    Task<List<VersionGroup>> GetAvailableVersions(IServiceProvider serviceProvider);

    Task DownloadVersion(Version version, string path, IServiceProvider serviceProvider);

    Task DoFinalSteps(string path, IServiceProvider serviceProvider);
    
    Task<string> GetAdditionalFlags(IServiceProvider serviceProvider);
}