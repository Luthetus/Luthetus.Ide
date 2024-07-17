using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceRegistryDefault : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public CompilerServiceRegistryDefault()
    {
        DefaultCompilerService = new CompilerService(null);
    }

    public CompilerService DefaultCompilerService { get; }
    
    public ImmutableDictionary<string, ICompilerService> Map => _map.ToImmutableDictionary();
    public ImmutableList<ICompilerService> CompilerServiceList => _map.Select(x => x.Value).ToImmutableList();

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
