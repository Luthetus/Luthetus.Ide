using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.Tutorials.RazorLib.CompilerServices;

public class CompilerServiceRegistry : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public IReadOnlyDictionary<string, ICompilerService> Map => _map;
    public IReadOnlyList<ICompilerService> CompilerServiceList => _map.Values.ToList();

    public CompilerServiceRegistry(ITextEditorService textEditorService, IClipboardService clipboardService)
    {
        CSharpCompilerService = new CSharpCompilerService(textEditorService, clipboardService);
        DefaultCompilerService = new CompilerServiceDoNothing();
        
        _map.Add(ExtensionNoPeriodFacts.C_SHARP_CLASS, CSharpCompilerService);
    }

    public CSharpCompilerService CSharpCompilerService { get; }
    public CompilerServiceDoNothing DefaultCompilerService { get; }

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
