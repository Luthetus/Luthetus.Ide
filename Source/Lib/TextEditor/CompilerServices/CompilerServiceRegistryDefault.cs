namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceRegistryDefault : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public CompilerServiceRegistryDefault()
    {
        CompilerServiceDoNothing = new CompilerServiceDoNothing();
    }

    public IReadOnlyList<ICompilerService> CompilerServiceList => _map.Values.ToList();

    public CompilerServiceDoNothing CompilerServiceDoNothing { get; }
    
    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return CompilerServiceDoNothing;
    }
}
