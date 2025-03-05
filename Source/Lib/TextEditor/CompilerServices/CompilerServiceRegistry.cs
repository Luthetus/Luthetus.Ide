using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceRegistryDefault : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public CompilerServiceRegistryDefault()
    {
        CompilerServiceDoNothing = new CompilerServiceDoNothing();
    }

    public CompilerServiceDoNothing CompilerServiceDoNothing { get; }
    
    public IReadOnlyDictionary<string, ICompilerService> Map => _map;
    public IReadOnlyList<ICompilerService> CompilerServiceList => _map.Select(x => x.Value).ToList();

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return CompilerServiceDoNothing;
    }
}
