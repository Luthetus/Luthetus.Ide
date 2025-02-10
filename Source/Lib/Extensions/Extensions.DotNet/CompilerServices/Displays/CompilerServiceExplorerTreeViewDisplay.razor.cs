using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CompilerServices.States;
using Luthetus.Extensions.DotNet.CompilerServices.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class CompilerServiceExplorerTreeViewDisplay : ComponentBase, IDisposable
{
	[Inject]
	private IState<CompilerServiceExplorerState> CompilerServiceExplorerStateWrap { get; set; } = null!;
	[Inject]
	private IState<TextEditorGroupState> TextEditorGroupStateWrap { get; set; } = null!;
	[Inject]
	private IDropdownService DropdownService { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;

	private CompilerServiceExplorerTreeViewKeyboardEventHandler _compilerServiceExplorerTreeViewKeymap = null!;
	private CompilerServiceExplorerTreeViewMouseEventHandler _compilerServiceExplorerTreeViewMouseEventHandler = null!;

	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

	private static bool _hasInitialized;

	protected override void OnInitialized()
	{
		CompilerServiceExplorerStateWrap.StateChanged += RerenderAfterEventWithArgs;
		TextEditorService.TextEditorStateChanged += RerenderAfterEvent;
		TextEditorGroupStateWrap.StateChanged += RerenderAfterEventWithArgs;

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
	
	private async void RerenderAfterEventWithArgs(object? sender, EventArgs e)
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
        DotNetBackgroundTaskApi.CompilerService.SetCompilerServiceExplorerTreeView();
	}

	public void Dispose()
	{
		CompilerServiceExplorerStateWrap.StateChanged -= RerenderAfterEventWithArgs;
		TextEditorService.TextEditorStateChanged -= RerenderAfterEvent;
		TextEditorGroupStateWrap.StateChanged -= RerenderAfterEventWithArgs;
	}
}