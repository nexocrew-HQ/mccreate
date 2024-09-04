using McCreate.App.Interfaces;

namespace McCreate.App.Implementations.Actions;

public class ListServersAction : IProgramAction
{
    public string Name { get; set; } = "List Created Servers";
    public async Task Execute(IServiceProvider serviceProvider)
    {
        
    }
}