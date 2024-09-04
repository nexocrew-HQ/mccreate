using MoonCore.Helpers;

namespace McCreate.App.Helpers;

public class ConfigHelper
{
    public ConfigHelper()
    {
    }

    public string Perform()
    {
        var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        char[] separators = new char[] { '/', '\\' };

        var configFilePath = PathBuilder.File(PathBuilder.Dir(
            applicationData.Split(separators, StringSplitOptions.None)),
            ".mccreate", "servers.json");
        
        
        if (!Directory.Exists(Path.GetDirectoryName(configFilePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configFilePath)!);
        }

        return configFilePath;
    }
}