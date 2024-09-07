using McCreate.App.Models;

namespace McCreate.App.Interfaces;

public interface IUpdateAction
{
    public string Name { get; set; }

    public Task Update(Server server, IServiceProvider serviceProvider);
}