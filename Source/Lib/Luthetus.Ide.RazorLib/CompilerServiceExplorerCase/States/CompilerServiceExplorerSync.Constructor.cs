using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
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
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerSync
{
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ILuthetusIdeComponentRenderers _luthetusIdeComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly XmlCompilerService _xmlCompilerService;
    private readonly DotNetSolutionCompilerService _dotNetSolutionCompilerService;
    private readonly CSharpProjectCompilerService _cSharpProjectCompilerService;
    private readonly CSharpCompilerService _cSharpCompilerService;
    private readonly RazorCompilerService _razorCompilerService;
    private readonly CssCompilerService _cssCompilerService;
    private readonly FSharpCompilerService _fSharpCompilerService;
    private readonly JavaScriptCompilerService _javaScriptCompilerService;
    private readonly TypeScriptCompilerService _typeScriptCompilerService;
    private readonly JsonCompilerService _jsonCompilerService;

    public CompilerServiceExplorerSync(
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
            ITreeViewService treeViewService,
            IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
            XmlCompilerService xmlCompilerService,
            DotNetSolutionCompilerService dotNetSolutionCompilerService,
            CSharpProjectCompilerService cSharpProjectCompilerService,
            CSharpCompilerService cSharpCompilerService,
            RazorCompilerService razorCompilerService,
            CssCompilerService cssCompilerService,
            FSharpCompilerService fSharpCompilerService,
            JavaScriptCompilerService javaScriptCompilerService,
            TypeScriptCompilerService typeScriptCompilerService,
            JsonCompilerService jsonCompilerService,
            IBackgroundTaskService backgroundTaskService,
            IDispatcher dispatcher)
    {
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _luthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        _treeViewService = treeViewService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _xmlCompilerService = xmlCompilerService;
        _dotNetSolutionCompilerService = dotNetSolutionCompilerService;
        _cSharpProjectCompilerService = cSharpProjectCompilerService;
        _cSharpCompilerService = cSharpCompilerService;
        _razorCompilerService = razorCompilerService;
        _cssCompilerService = cssCompilerService;
        _fSharpCompilerService = fSharpCompilerService;
        _javaScriptCompilerService = javaScriptCompilerService;
        _typeScriptCompilerService = typeScriptCompilerService;
        _jsonCompilerService = jsonCompilerService;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}