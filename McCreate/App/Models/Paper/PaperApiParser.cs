using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace McCreate.App.Models.Paper;

public class PaperApiParser
{
    public class VersionApi
    {
        [JsonProperty("version_groups")]
        public List<string> VersionGroups { get; set; }

        [JsonProperty("versions")]
        public List<string> Versions { get; set; } = new();
    }
    
    public class DownloadApi
    {
        [JsonProperty("builds")]
        public List<Build> Builds { get; set; }

        public class Build
        {
            [JsonProperty("build")]
            public int BuildNumber { get; set; }

            [JsonProperty("time")]
            public DateTime Time { get; set; }

            [JsonProperty("channel")]
            public string Channel { get; set; }
            
            [JsonProperty("downloads")]
            public DownloadsModel Downloads { get; set; }
            
            public class DownloadsModel
            {
                [JsonProperty("application")]
                public ApplicationDownload Application { get; set; }
                
                public class ApplicationDownload
                {
                    [JsonProperty("name")]
                    public string Name { get; set; }

                    [JsonProperty("sha256")]
                    public string Sha256 { get; set; }
                }
            }

            
        }
    }
}