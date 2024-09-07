using McCreate.App.Helpers;
using McCreate.App.Interfaces;
using McCreate.App.Models;

namespace McCreate.App.Implementations.UpdateActions;

public class FlagsUpdate : IUpdateAction
{
    public string Name { get; set; } = "Change Flags";
    
    public async Task Update(Server server, IServiceProvider serviceProvider)
    {
        if (server.Software.HasAdditionalFlags)
        {
            server.AdditionalFlags = await server.Software.GetAdditionalFlags(serviceProvider);

            await ServerHelper.CreateStartupFiles(server);
            
            AnsiHelper.Success("Successfully updated server flags.");
        }
        else
        {
            AnsiHelper.Info("The selected server software does not have any additional flags.");
        }
    }
    
}