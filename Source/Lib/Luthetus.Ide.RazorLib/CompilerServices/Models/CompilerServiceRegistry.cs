using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
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
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.CompilerServices.Models;

public class CompilerServiceRegistry : ICompilerServiceRegistry
{
    private Dictionary<string, ICompilerService> _map { get; } = new();

    public ImmutableDictionary<string, ICompilerService> Map => _map.ToImmutableDictionary();

    public CompilerServiceRegistry(
        ITextEditorService textEditorService,
        IBackgroundTaskService backgroundTaskService,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher)
    {
        CSharpCompilerService = new CSharpCompilerService(textEditorService, backgroundTaskService, dispatcher);
        CSharpProjectCompilerService = new CSharpProjectCompilerService(textEditorService, backgroundTaskService, dispatcher);
        CssCompilerService = new CssCompilerService(textEditorService, backgroundTaskService, dispatcher);
        DotNetSolutionCompilerService = new DotNetSolutionCompilerService(textEditorService, backgroundTaskService, environmentProvider, dispatcher);
        FSharpCompilerService = new FSharpCompilerService(textEditorService, backgroundTaskService, dispatcher);
        JavaScriptCompilerService = new JavaScriptCompilerService(textEditorService, backgroundTaskService, dispatcher);
        JsonCompilerService = new JsonCompilerService(textEditorService, backgroundTaskService, dispatcher);
        RazorCompilerService = new RazorCompilerService(textEditorService, backgroundTaskService, CSharpCompilerService, environmentProvider, dispatcher);
        TypeScriptCompilerService = new TypeScriptCompilerService(textEditorService, backgroundTaskService, dispatcher);
        XmlCompilerService = new XmlCompilerService(textEditorService, backgroundTaskService, dispatcher);
        DefaultCompilerService = new TextEditorDefaultCompilerService(textEditorService, backgroundTaskService, dispatcher);
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
    public TextEditorDefaultCompilerService DefaultCompilerService { get; }

    public ICompilerService GetCompilerService(string extensionNoPeriod)
    {
        if (_map.TryGetValue(extensionNoPeriod, out var compilerService))
            return compilerService;

        return DefaultCompilerService;
    }
}
