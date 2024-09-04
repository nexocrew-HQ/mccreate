using McCreate.App.Interfaces;
using Newtonsoft.Json;

namespace McCreate.App.Models;

public class Server
{
    [JsonProperty("ServerSoftware")]
    public IServerSoftware Software { get; set; }
}