namespace McCreate.App.Interfaces;

public interface IProgramAction
{
    string Name { get; set; }
    
    string Description { get; set; }

    Task Execute(IServiceProvider serviceProvider);

}