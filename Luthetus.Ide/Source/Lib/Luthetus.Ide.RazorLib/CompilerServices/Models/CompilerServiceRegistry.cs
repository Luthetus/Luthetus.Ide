using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.CompilerServices.Lang.C;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.FSharp;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.CompilerServices.Models;

public class CompilerServiceRegistry : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ILuthCompilerService> _map = new();

    public ImmutableDictionary<string, ILuthCompilerService> Map => _map.ToImmutableDictionary();

    public CompilerServiceRegistry(
        ITextEditorService textEditorService,
        IEnvironmentProvider environmentProvider)
    {
        CSharpCompilerService = new CSharpCompilerService(textEditorService);
        CSharpProjectCompilerService = new CSharpProjectCompilerService(textEditorService);
        CssCompilerService = new CssCompilerService(textEditorService);
        DotNetSolutionCompilerService = new DotNetSolutionCompilerService(textEditorService);
        FSharpCompilerService = new FSharpCompilerService(textEditorService);
        JavaScriptCompilerService = new JavaScriptCompilerService(textEditorService);
        JsonCompilerService = new JsonCompilerService(textEditorService);
        RazorCompilerService = new RazorCompilerService(textEditorService, CSharpCompilerService, environmentProvider);
        TypeScriptCompilerService = new TypeScriptCompilerService(textEditorService);
        XmlCompilerService = new XmlCompilerService(textEditorService);
        CCompilerService = new CCompilerService(textEditorService);
        TerminalCompilerService = new TerminalCompilerService(textEditorService);
        DefaultCompilerService = new LuthCompilerService(textEditorService);

        _map.Add(ExtensionNoPeriodFacts.HTML, XmlCompilerService);
        _map.Add(ExtensionNoPeriodFacts.XML, XmlCompilerService);
        _map.Add(ExtensionNoPeriodFacts.C_SHARP_PROJECT, CSharpProjectCompilerService);
        _map.Add(ExtensionNoPeriodFacts.C_SHARP_CLASS, CSharpCompilerService);
        _map.Add(ExtensionNoPeriodFacts.RAZOR_CODEBEHIND, CSharpCompilerService);
        _map.Add(ExtensionNoPeriodFacts.RAZOR_MARKUP, RazorCompilerService);
        _map.Add(ExtensionNoPeriodFacts.CSHTML_CLASS, RazorCompilerService);
        _map.Add(ExtensionNoPeriodFacts.CSS, CssCompilerService);
        _map.Add(ExtensionNoPeriodFacts.JAVA_SCRIPT, JavaScriptCompilerService);
        _map.Add(ExtensionNoPeriodFacts.JSON, JsonCompilerService);
        _map.Add(ExtensionNoPeriodFacts.TYPE_SCRIPT, TypeScriptCompilerService);
        _map.Add(ExtensionNoPeriodFacts.F_SHARP, FSharpCompilerService);
        _map.Add(ExtensionNoPeriodFacts.DOT_NET_SOLUTION, DotNetSolutionCompilerService);
        _map.Add(ExtensionNoPeriodFacts.C, CCompilerService);
        _map.Add(ExtensionNoPeriodFacts.H, CCompilerService);
        _map.Add(ExtensionNoPeriodFacts.CPP, CCompilerService);
        _map.Add(ExtensionNoPeriodFacts.HPP, CCompilerService);
        _map.Add(ExtensionNoPeriodFacts.TERMINAL, TerminalCompilerService);
    }

    public CSharpCompilerService CSharpCompilerService { get; }
    public CSharpProjectCompilerService CSharpProjectCompilerService { get; }
    public CssCompilerService CssCompilerService { get; }
    public DotNetSolutionCompilerService DotNetSolutionCompilerService { get; }
    public FSharpCompilerService FSharpCompilerService { get; }
    public JavaScriptCompilerService JavaScriptCompilerService { get; }
    public JsonCompilerService JsonCompilerService { get; }
    public RazorCompilerService RazorCompilerService { get; }
    public TypeScriptCompilerService TypeScriptCompilerService { get; }
    public XmlCompilerService XmlCompilerService { get; }
    public CCompilerService CCompilerService { get; }
    public TerminalCompilerService TerminalCompilerService { get; }
    public LuthCompilerService DefaultCompilerService { get; }

    public ILuthCompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
