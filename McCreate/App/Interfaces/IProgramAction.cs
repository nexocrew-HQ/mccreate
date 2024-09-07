namespace McCreate.App.Interfaces;

public interface IProgramAction
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public Task Execute(IServiceProvider serviceProvider);

}