using McCreate.App.Interfaces;

namespace McCreate.App.Implementations;

public class CreateServerAction : IProgramAction
{
    public string Name { get; set; } = "Create new Server";

    public async Task Execute(IServiceProvider serviceProvider)
    {
        Console.WriteLine("TEST");
    }
}