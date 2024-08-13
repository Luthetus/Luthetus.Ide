using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.Tutorials.RazorLib.CompilerServices;

public class CompilerServiceRegistry : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public ImmutableDictionary<string, ICompilerService> Map => _map.ToImmutableDictionary();
    public ImmutableList<ICompilerService> CompilerServiceList => _map.Select(x => x.Value).ToImmutableList();

    public CompilerServiceRegistry(ITextEditorService textEditorService)
    {
        CSharpCompilerService = new CSharpCompilerService(textEditorService);
        DefaultCompilerService = new CompilerService(textEditorService);
        
        _map.Add(ExtensionNoPeriodFacts.C_SHARP_CLASS, CSharpCompilerService);
    }

    public CSharpCompilerService CSharpCompilerService { get; }
    public CompilerService DefaultCompilerService { get; }

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
