using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace mccreate.App.Models.ServerSoftwareModels;

public class PurpurApiParser
{
    public class VersionApi
    {
        [JsonProperty("versions")]
        public List<string> Versions { get; set; }
    }

    public class DownloadApi
    {
        public class BuildsApi
        {
            [JsonProperty("builds")]
            public BuildsData Builds { get; set; }

            public class BuildsData
            {
                [JsonProperty("latest")]
                public string LatestBuild { get; set; }
                
                [JsonProperty("all")]
                public List<string> AllBuils { get; set; }
            }
        }

        public class BuildSuccess
        {
            [JsonProperty("result")]
            public string Result { get; set; }
        }
    }
    
}