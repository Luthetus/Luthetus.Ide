using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CompilerServices.States;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceIdeApi
{
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly IState<CompilerServiceExplorerState> _compilerServiceExplorerStateWrap;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly ITreeViewService _treeViewService;
	private readonly IDispatcher _dispatcher;

	public CompilerServiceIdeApi(
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
		IBackgroundTaskService backgroundTaskService,
		IState<CompilerServiceExplorerState> compilerServiceExplorerStateWrap,
		ICompilerServiceRegistry compilerServiceRegistry,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		ITreeViewService treeViewService,
		IDispatcher dispatcher)
	{
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_compilerServiceExplorerStateWrap = compilerServiceExplorerStateWrap;
		_compilerServiceRegistry = compilerServiceRegistry;
		_ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_treeViewService = treeViewService;
		_dispatcher = dispatcher;
	}

    /// <summary>
    /// TODO: Iterate over _compilerServiceExplorerStateWrap.Value.CompilerServiceList instead...
	///       ...of invoking 'GetCompilerService' with hardcoded values.
    /// </summary>
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
					_compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.XML),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.XML).GetType(),
					"XML",
					true);

				var dotNetSolutionCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION).GetType(),
					".NET Solution",
					true);

				var cSharpProjectCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_PROJECT),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_PROJECT).GetType(),
					"C# Project",
					true);

				var cSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS).GetType(),
					"C#",
					true);

				var razorCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.RAZOR_MARKUP),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.RAZOR_MARKUP).GetType(),
					"Razor",
					true);

				var cssCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.CSS),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.CSS).GetType(),
					"Css",
					true);

				var fSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.F_SHARP),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.F_SHARP).GetType(),
					"F#",
					true);

				var javaScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JAVA_SCRIPT),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JAVA_SCRIPT).GetType(),
					"JavaScript",
					true);

				var typeScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TYPE_SCRIPT),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TYPE_SCRIPT).GetType(),
					"TypeScript",
					true);

				var jsonCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JSON),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JSON).GetType(),
					"JSON",
					true);

				var terminalCompilerServiceWatchWindowObject = new WatchWindowObject(
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL),
                    _compilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL).GetType(),
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
