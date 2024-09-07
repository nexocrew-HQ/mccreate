using McCreate.App.Models;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Interfaces;

public interface IServerSoftware
{
    public string Name { get; set; }
    
    public string UniqueId { get; set; }

    public Task<List<VersionGroup>> GetAvailableVersions(IServiceProvider serviceProvider);

    public Task DownloadVersion(Version version, string path, IServiceProvider serviceProvider);

    public Task DoFinalSteps(string path, IServiceProvider serviceProvider);
    
    public Task<string> GetAdditionalFlags(IServiceProvider serviceProvider);
}