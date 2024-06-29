using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitChangesTreeViewDisplay : ComponentBase
{
    /// <summary>
    /// Awkwardly, the <see cref="GitTreeViewKeyboardEventHandler"/> constructor needs this,
    /// meanwhile this component is receiving <see cref="States.GitState"/> as a cascading parameter.
    /// This should be written differently (2024-05-02).
    /// </summary>
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
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
    private GitTreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private GitTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

    protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new GitTreeViewKeyboardEventHandler(
            TreeViewService,
            BackgroundTaskService,
            GitStateWrap,
            Dispatcher);

        _treeViewMouseEventHandler = new GitTreeViewMouseEventHandler(
            TreeViewService,
            BackgroundTaskService,
            GitStateWrap,
            Dispatcher);

        base.OnInitialized();
    }

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
		var dropdownRecord = new DropdownRecord(
			GitChangesContextMenu.ContextMenuEventDropdownKey,
			treeViewCommandArgs.ContextMenuFixedPosition.LeftPositionInPixels,
			treeViewCommandArgs.ContextMenuFixedPosition.TopPositionInPixels,
			typeof(GitChangesContextMenu),
			new Dictionary<string, object?>
			{
				{
					nameof(GitChangesContextMenu.TreeViewCommandArgs),
					treeViewCommandArgs
				}
			});

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
    }
}