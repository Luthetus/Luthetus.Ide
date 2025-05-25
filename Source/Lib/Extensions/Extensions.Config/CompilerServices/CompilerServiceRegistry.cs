using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Python;
using Luthetus.CompilerServices.C;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Css;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.FSharp;
using Luthetus.CompilerServices.JavaScript;
using Luthetus.CompilerServices.Json;
using Luthetus.CompilerServices.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.TypeScript;
using Luthetus.CompilerServices.Xml;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.Config.CompilerServices;

public class ConfigCompilerServiceRegistry : ICompilerServiceRegistry
{
    private readonly Dictionary<string, ICompilerService> _map = new();

    public IReadOnlyDictionary<string, ICompilerService> Map => _map;
    public IReadOnlyList<ICompilerService> CompilerServiceList => _map.Select(x => x.Value).ToList();

    public ConfigCompilerServiceRegistry(
        TextEditorService textEditorService,
        IEnvironmentProvider environmentProvider,
        ITerminalService terminalService,
        IClipboardService clipboardService)
    {
        CSharpCompilerService = new CSharpCompilerService(textEditorService, clipboardService);
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
        PythonCompilerService = new PythonCompilerService(textEditorService);
        TerminalCompilerService = new TerminalCompilerService(textEditorService, terminalService);
        DefaultCompilerService = new CompilerServiceDoNothing();

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
        _map.Add(ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X, DotNetSolutionCompilerService);
        _map.Add(ExtensionNoPeriodFacts.C, CCompilerService);
        _map.Add(ExtensionNoPeriodFacts.PYTHON, PythonCompilerService);
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
    public PythonCompilerService PythonCompilerService { get; }
    public TerminalCompilerService TerminalCompilerService { get; }
    public CompilerServiceDoNothing DefaultCompilerService { get; }

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
