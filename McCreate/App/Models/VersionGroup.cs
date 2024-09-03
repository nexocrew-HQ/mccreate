namespace McCreate.App.Models;

public class VersionGroup
{
    public string PrimaryVersion { get; set; }

    public List<Version> SubVersions { get; set; } = new();
}