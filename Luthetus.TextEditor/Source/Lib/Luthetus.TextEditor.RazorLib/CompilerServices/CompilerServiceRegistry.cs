using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceRegistryDefault : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public CompilerServiceRegistryDefault()
    {
        DefaultCompilerService = new TextEditorDefaultCompilerService();
    }

    public TextEditorDefaultCompilerService DefaultCompilerService { get; }
    
    public ImmutableDictionary<string, ICompilerService> Map => _map.ToImmutableDictionary();

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
