using System.Collections.Concurrent;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceIdeApi : IBackgroundTaskGroup
{
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly ICompilerServiceExplorerService _compilerServiceExplorerService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IIdeComponentRenderers _ideComponentRenderers;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly ITreeViewService _treeViewService;

	public CompilerServiceIdeApi(
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
		BackgroundTaskService backgroundTaskService,
		ICompilerServiceExplorerService compilerServiceExplorerService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IIdeComponentRenderers ideComponentRenderers,
		ICommonComponentRenderers commonComponentRenderers,
		ITreeViewService treeViewService)
	{
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
		_backgroundTaskService = backgroundTaskService;
		_compilerServiceExplorerService = compilerServiceExplorerService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_ideComponentRenderers = ideComponentRenderers;
		_commonComponentRenderers = commonComponentRenderers;
		_treeViewService = treeViewService;
	}

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<CompilerServiceIdeWorkKind> _workQueue = new();

    public void Enqueue(CompilerServiceIdeWorkKind workKind)
	{
        _workQueue.Enqueue(workKind);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
    }

    /// <summary>
    /// TODO: Iterate over _compilerServiceExplorerStateWrap.Value.CompilerServiceList instead...
	///       ...of invoking 'GetCompilerService' with hardcoded values.
    /// </summary>
    public async ValueTask Do_SetCompilerServiceExplorerTreeView()
	{
		var compilerServiceExplorerState = _compilerServiceExplorerService.GetCompilerServiceExplorerState();

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
			_treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				rootNode,
				new List<TreeViewNoType> { rootNode }));
		}
		else
		{
			_treeViewService.ReduceWithRootNodeAction(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				rootNode);

			_treeViewService.ReduceSetActiveNodeAction(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				rootNode,
				true,
				false);
		}

		_dotNetBackgroundTaskApi.CompilerServiceExplorerService.ReduceNewAction(inCompilerServiceExplorerState =>
			new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model));
	}

    public ValueTask HandleEvent()
    {
        if (!_workQueue.TryDequeue(out CompilerServiceIdeWorkKind workKind))
            return ValueTask.CompletedTask;

        switch (workKind)
        {
            case CompilerServiceIdeWorkKind.SetCompilerServiceExplorerTreeView:
                return Do_SetCompilerServiceExplorerTreeView();
            default:
                Console.WriteLine($"{nameof(CompilerServiceIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
        }
    }
}
