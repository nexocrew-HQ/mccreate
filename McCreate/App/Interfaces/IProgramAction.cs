namespace McCreate.App.Interfaces;

public interface IProgramAction
{
    string Name { get; set; }

    Task Execute(IServiceProvider serviceProvider);

}