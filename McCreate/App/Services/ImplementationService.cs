namespace mccreate.App.Services;

public class ImplementationService
{
    
    private readonly Dictionary<Type, List<object>> InterfaceImplementations = new();
    

    public ImplementationService()
    {

    }

    public void RegisterImplementation<T>(T implementation) where T : class
    {
        var interfaceType = typeof(T);

        lock (InterfaceImplementations)
        {
            if(!InterfaceImplementations.ContainsKey(interfaceType))
                InterfaceImplementations.Add(interfaceType, new());

            InterfaceImplementations[interfaceType].Add(implementation);
        }
    }

    public void RegisterImplementation<T, TImpl>() where TImpl : T where T : class
    {
        var impl = Activator.CreateInstance<TImpl>();

        RegisterImplementation<T>(impl);
    }

    public T[] GetImplementations<T>() where T : class
    {
        var interfaceType = typeof(T);

        lock (InterfaceImplementations)
        {
            if (!InterfaceImplementations.ContainsKey(interfaceType))
                return [];

            return InterfaceImplementations[interfaceType]
                .Select(x => (T)x)
                .ToArray();
        }
    }

    public async Task ExecuteImplementations<T>(Func<T, Task> func) where T : class
    {
        var implementations = GetImplementations<T>();

        foreach (var implementation in implementations)
            await func.Invoke(implementation);
    }
}