using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class CompilerServiceIdeApi
{
    private readonly LuthetusCommonApi _commonApi;
    private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	private readonly ICompilerServiceExplorerService _compilerServiceExplorerService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IIdeComponentRenderers _ideComponentRenderers;

	public CompilerServiceIdeApi(
		LuthetusCommonApi commonApi,
        DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
		ICompilerServiceExplorerService compilerServiceExplorerService,
		ICompilerServiceRegistry compilerServiceRegistry,
		IIdeComponentRenderers ideComponentRenderers)
	{
		_commonApi = commonApi;
        _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
		_compilerServiceExplorerService = compilerServiceExplorerService;
		_compilerServiceRegistry = compilerServiceRegistry;
		_ideComponentRenderers = ideComponentRenderers;
	}

    /// <summary>
    /// TODO: Iterate over _compilerServiceExplorerStateWrap.Value.CompilerServiceList instead...
	///       ...of invoking 'GetCompilerService' with hardcoded values.
    /// </summary>
    public void SetCompilerServiceExplorerTreeView()
	{
		_commonApi.BackgroundTaskApi.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BackgroundTaskFacts.ContinuousQueueKey,
			"Set CompilerServiceExplorer TreeView",
			async () =>
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
					new TreeViewReflection(xmlCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(dotNetSolutionCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(cSharpProjectCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(cSharpCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(razorCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(cssCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(fSharpCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(javaScriptCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(typeScriptCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(jsonCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi),
					new TreeViewReflection(terminalCompilerServiceWatchWindowObject, true, false, _commonApi.ComponentRendererApi));

				await rootNode.LoadChildListAsync().ConfigureAwait(false);

				if (!_commonApi.TreeViewApi.TryGetTreeViewContainer(
						CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
						out var treeViewState))
				{
					_commonApi.TreeViewApi.ReduceRegisterContainerAction(new TreeViewContainer(
						CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
						rootNode,
						new() { rootNode }));
				}
				else
				{
					_commonApi.TreeViewApi.ReduceWithRootNodeAction(
						CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
						rootNode);

					_commonApi.TreeViewApi.ReduceSetActiveNodeAction(
						CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
						rootNode,
						true,
						false);
				}

				_dotNetBackgroundTaskApi.CompilerServiceExplorerService.ReduceNewAction(inCompilerServiceExplorerState =>
					new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model));
			});
	}
}
