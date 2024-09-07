using McCreate.App.Models;
using Newtonsoft.Json;

namespace McCreate.App.Configuration;

public class ConfigModel
{
    public List<ConfigServer> Servers { get; set; } = new();
}