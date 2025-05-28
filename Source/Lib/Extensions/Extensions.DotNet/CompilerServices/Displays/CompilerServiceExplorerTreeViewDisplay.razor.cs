using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class CompilerServiceExplorerTreeViewDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;
	[Inject]
	private TextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private BackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

	private CompilerServiceExplorerTreeViewKeyboardEventHandler _compilerServiceExplorerTreeViewKeymap = null!;
	private CompilerServiceExplorerTreeViewMouseEventHandler _compilerServiceExplorerTreeViewMouseEventHandler = null!;

	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsService.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));

	private static bool _hasInitialized;

	protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.CompilerServiceExplorerService.CompilerServiceExplorerStateChanged += RerenderAfterEvent;
		TextEditorService.TextEditorStateChanged += RerenderAfterEvent;

		_compilerServiceExplorerTreeViewKeymap = new CompilerServiceExplorerTreeViewKeyboardEventHandler(
			IdeBackgroundTaskApi,
			TreeViewService,
			BackgroundTaskService);

		_compilerServiceExplorerTreeViewMouseEventHandler = new CompilerServiceExplorerTreeViewMouseEventHandler(
			IdeBackgroundTaskApi,
			TreeViewService,
			BackgroundTaskService);

		base.OnInitialized();
	}

	protected override Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			if (!_hasInitialized)
			{
				_hasInitialized = true;
				ReloadOnClick();
			}
		}

		return base.OnAfterRenderAsync(firstRender);
	}

	private async void RerenderAfterEvent()
	{
		await InvokeAsync(StateHasChanged);
	}

	private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
	{
		var dropdownRecord = new DropdownRecord(
			CompilerServiceExplorerTreeViewContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(CompilerServiceExplorerTreeViewContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(CompilerServiceExplorerTreeViewContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			restoreFocusOnClose: null);

		DropdownService.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}

	private void ReloadOnClick()
	{
		BackgroundTaskService.Continuous_EnqueueGroup(new BackgroundTask(
			Key<IBackgroundTaskGroup>.Empty,
			Do_SetCompilerServiceExplorerTreeView));
	}
	
	/// <summary>
    /// TODO: Iterate over _compilerServiceExplorerStateWrap.Value.CompilerServiceList instead...
	///       ...of invoking 'GetCompilerService' with hardcoded values.
    /// </summary>
    public async ValueTask Do_SetCompilerServiceExplorerTreeView()
	{
		var compilerServiceExplorerState = DotNetBackgroundTaskApi.CompilerServiceExplorerService.GetCompilerServiceExplorerState();

		var xmlCompilerServiceWatchWindowObject = new WatchWindowObject(
			CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.XML),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.XML).GetType(),
			"XML",
			true);

		var dotNetSolutionCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.DOT_NET_SOLUTION).GetType(),
			".NET Solution",
			true);

		var cSharpProjectCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_PROJECT),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_PROJECT).GetType(),
			"C# Project",
			true);

		var cSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS).GetType(),
			"C#",
			true);

		var razorCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.RAZOR_MARKUP),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.RAZOR_MARKUP).GetType(),
			"Razor",
			true);

		var cssCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.CSS),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.CSS).GetType(),
			"Css",
			true);

		var fSharpCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.F_SHARP),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.F_SHARP).GetType(),
			"F#",
			true);

		var javaScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JAVA_SCRIPT),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JAVA_SCRIPT).GetType(),
			"JavaScript",
			true);

		var typeScriptCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TYPE_SCRIPT),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TYPE_SCRIPT).GetType(),
			"TypeScript",
			true);

		var jsonCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JSON),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.JSON).GetType(),
			"JSON",
			true);

		var terminalCompilerServiceWatchWindowObject = new WatchWindowObject(
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL),
            CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.TERMINAL).GetType(),
			"Terminal",
			true);

		var rootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
			new TreeViewReflection(xmlCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(dotNetSolutionCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(cSharpProjectCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(cSharpCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(razorCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(cssCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(fSharpCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(javaScriptCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(typeScriptCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(jsonCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers),
			new TreeViewReflection(terminalCompilerServiceWatchWindowObject, true, false, CommonComponentRenderers));

		await rootNode.LoadChildListAsync().ConfigureAwait(false);

		if (!TreeViewService.TryGetTreeViewContainer(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				out var treeViewState))
		{
			TreeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				rootNode,
				new List<TreeViewNoType> { rootNode }));
		}
		else
		{
			TreeViewService.ReduceWithRootNodeAction(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				rootNode);

			TreeViewService.ReduceSetActiveNodeAction(
				CompilerServiceExplorerState.TreeViewCompilerServiceExplorerContentStateKey,
				rootNode,
				true,
				false);
		}

		DotNetBackgroundTaskApi.CompilerServiceExplorerService.ReduceNewAction(inCompilerServiceExplorerState =>
			new CompilerServiceExplorerState(inCompilerServiceExplorerState.Model));
	}

	public void Dispose()
	{
		DotNetBackgroundTaskApi.CompilerServiceExplorerService.CompilerServiceExplorerStateChanged -= RerenderAfterEvent;
		TextEditorService.TextEditorStateChanged -= RerenderAfterEvent;
	}
}