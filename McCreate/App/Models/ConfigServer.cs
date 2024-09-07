using McCreate.App.Interfaces;
using Newtonsoft.Json;

namespace McCreate.App.Models;

public class ConfigServer
{
    public string SoftwareId { get; set; }
    
    public Version Version { get; set; }
    
    public int MemoryInMegabytes { get; set; }
    
    public string AdditionalFlags { get; set; }
    
    public string Path { get; set; }
}