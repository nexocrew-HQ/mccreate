using McCreate.App.Models;
using Newtonsoft.Json;

namespace McCreate.App.Configuration;

public class ConfigModel
{
    public List<Server> Servers { get; set; } = new();
}