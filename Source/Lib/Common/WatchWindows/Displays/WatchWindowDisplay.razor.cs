using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Displays;

/// <summary>
/// TODO: SphagettiCode - TextEditor css classes are referenced in this
/// tree view? Furthermore, because of this, the Text Editor css classes are not being
/// set to the correct theme (try a non visual studio clone theme -- it doesn't work). (2023-09-19)
/// </summary>
public partial class WatchWindowDisplay : FluxorComponent
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public WatchWindowObject WatchWindowObject { get; set; } = null!;

    public static Key<TreeViewContainer> TreeViewContainerKey { get; } = Key<TreeViewContainer>.NewKey();
    public static Key<DropdownRecord> WatchWindowContextMenuDropdownKey { get; } = Key<DropdownRecord>.NewKey();

    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
    private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private bool _disposed;

    protected override void OnInitialized()
    {
        _treeViewMouseEventHandler = new(TreeViewService, BackgroundTaskService);
        _treeViewKeyboardEventHandler = new(TreeViewService, BackgroundTaskService);
        base.OnInitialized();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (!TreeViewService.TryGetTreeViewContainer(TreeViewContainerKey, out var treeViewContainer))
            {
                var rootNode = new TreeViewReflection(
                    WatchWindowObject,
                    true,
                    false,
                    CommonComponentRenderers);

                TreeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                    TreeViewContainerKey,
                    rootNode,
                    new TreeViewNoType[] { rootNode }.ToImmutableList()));
            }
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
		var dropdownRecord = new DropdownRecord(
			WatchWindowContextMenuDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(WatchWindowContextMenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(WatchWindowContextMenuDisplay.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			},
			treeViewCommandArgs.RestoreFocusToTreeView);

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            TreeViewService.DisposeTreeViewContainer(TreeViewContainerKey);
        }

        base.Dispose(disposing);
    }
}