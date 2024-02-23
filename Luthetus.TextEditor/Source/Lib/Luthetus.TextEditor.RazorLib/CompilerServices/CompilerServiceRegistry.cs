using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceRegistryDefault : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ILuthCompilerService> _map = new();

    public CompilerServiceRegistryDefault()
    {
        DefaultCompilerService = new LuthCompilerService(null);
    }

    public LuthCompilerService DefaultCompilerService { get; }
    
    public ImmutableDictionary<string, ILuthCompilerService> Map => _map.ToImmutableDictionary();

    public ILuthCompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
