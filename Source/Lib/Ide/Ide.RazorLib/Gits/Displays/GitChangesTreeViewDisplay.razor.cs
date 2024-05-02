using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitChangesTreeViewDisplay : ComponentBase
{
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;
    private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

    protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new TreeViewKeyboardEventHandler(
            TreeViewService,
            BackgroundTaskService);

        _treeViewMouseEventHandler = new TreeViewMouseEventHandler(
            TreeViewService,
            BackgroundTaskService);

        base.OnInitialized();
    }

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

        // The order of 'StateHasChanged(...)' and 'AddActiveDropdownKey(...)' is important.
        // The ChildContent renders nothing, unless the provider of the child content
        // re-renders now that there is a given '_mostRecentTreeViewContextMenuCommandArgs'
        await InvokeAsync(StateHasChanged);

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            GitChangesContextMenu.ContextMenuEventDropdownKey));
    }
}