using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.Models;

public class CompilerServiceIdeApi
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
    private readonly CompilerServiceRegistry _compilerServiceRegistry;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IDispatcher _dispatcher;

    public CompilerServiceIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IBackgroundTaskService backgroundTaskService,
        IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
        CompilerServiceRegistry compilerServiceRegistry,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _backgroundTaskService = backgroundTaskService;
        _compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
        _compilerServiceRegistry = compilerServiceRegistry;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _dispatcher = dispatcher;
    }

    public void SetCompilerServiceExplorerTreeView()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set CompilerServiceExplorer TreeView",
            async () =>
            {
                var compilerServiceExplorerState = _compilerServiceExplorerStateWrap.Value;

                var xmlCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.XmlCompilerService,
                    _compilerServiceRegistry.XmlCompilerService.GetType(),
                    "XML",
                    true);

                var dotNetSolutionCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.DotNetSolutionCompilerService,
                    _compilerServiceRegistry.DotNetSolutionCompilerService.GetType(),
                    ".NET Solution",
                    true);

                var cSharpProjectCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CSharpProjectCompilerService,
                    _compilerServiceRegistry.CSharpProjectCompilerService.GetType(),
                    "C# Project",
                    true);

                var cSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CSharpCompilerService,
                    _compilerServiceRegistry.CSharpCompilerService.GetType(),
                    "C#",
                    true);

                var razorCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.RazorCompilerService,
                    _compilerServiceRegistry.RazorCompilerService.GetType(),
                    "Razor",
                    true);

                var cssCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.CssCompilerService,
                    _compilerServiceRegistry.CssCompilerService.GetType(),
                    "Css",
                    true);

                var fSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.FSharpCompilerService,
                    _compilerServiceRegistry.FSharpCompilerService.GetType(),
                    "F#",
                    true);

                var javaScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.JavaScriptCompilerService,
                    _compilerServiceRegistry.JavaScriptCompilerService.GetType(),
                    "JavaScript",
                    true);

                var typeScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.TypeScriptCompilerService,
                    _compilerServiceRegistry.TypeScriptCompilerService.GetType(),
                    "TypeScript",
                    true);

                var jsonCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.JsonCompilerService,
                    _compilerServiceRegistry.JsonCompilerService.GetType(),
                    "JSON",
                    true);

                var terminalCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.TerminalCompilerService,
                    _compilerServiceRegistry.TerminalCompilerService.GetType(),
                    "Terminal",
                    true);

                var rootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
                    new TreeViewReflection(xmlCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(dotNetSolutionCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(cSharpProjectCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(cSharpCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(razorCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(cssCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(fSharpCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(javaScriptCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(typeScriptCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(jsonCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers),
                    new TreeViewReflection(terminalCompilerServiceWatchWindowObject, true, false, _commonComponentRenderers));

                await rootNode.LoadChildListAsync().ConfigureAwait(false);

                if (!_treeViewService.TryGetTreeViewContainer(
                        CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
                        out var treeViewState))
                {
                    _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                        CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        new TreeViewNoType[] { rootNode }.ToImmutableList()));
                }
                else
                {
                    _treeViewService.SetRoot(
                        CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode);

                    _treeViewService.SetActiveNode(
                        CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
                        rootNode,
                        true,
                        false);
                }

                _dispatcher.Dispatch(new CompilerServiceExplorerState.NewAction(inCompilerServiceExplorerState =>
                    new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model)));
            });
    }
}
