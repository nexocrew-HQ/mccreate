using McCreate.App.Models;
using Version = McCreate.App.Models.Version;

namespace McCreate.App.Helpers;

public class VersionHelper
{
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