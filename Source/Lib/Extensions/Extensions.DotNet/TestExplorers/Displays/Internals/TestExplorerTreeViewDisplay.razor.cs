using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.Displays.Internals;

public partial class TestExplorerTreeViewDisplay : ComponentBase
{
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
	private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	private IServiceProvider ServiceProvider { get; set; } = null!;

	[CascadingParameter]
	public TestExplorerRenderBatchValidated RenderBatch { get; set; } = null!;

	[Parameter, EditorRequired]
	public ElementDimensions ElementDimensions { get; set; } = null!;

	private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
	private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
		RenderBatch.AppOptionsState.Options.IconSizeInPixels * (2.0 / 3.0));

	protected override void OnInitialized()
	{
		_treeViewKeyboardEventHandler = new TestExplorerTreeViewKeyboardEventHandler(
			CommonComponentRenderers,
			Dispatcher,
			CompilerServiceRegistry,
			TextEditorService,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		_treeViewMouseEventHandler = new TestExplorerTreeViewMouseEventHandler(
			CommonComponentRenderers,
			Dispatcher,
			CompilerServiceRegistry,
			TextEditorService,
			ServiceProvider,
			TreeViewService,
			BackgroundTaskService);

		base.OnInitialized();
	}

	private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
	{
		var dropdownRecord = new DropdownRecord(
			TestExplorerContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(TestExplorerContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(TestExplorerContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			restoreFocusOnClose: null);

		Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
		return Task.CompletedTask;
	}
}