using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;

namespace McCreate.App.Implementations.UpdateActions;

public class RestoreStartupFiles : IUpdateAction
{
    public string Name { get; set; } = "Restore startup files";
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        await ServerHelper.CreateStartupFiles(server);
        
        AnsiHelper.Success("Successfully recreated startup files");
    }
}