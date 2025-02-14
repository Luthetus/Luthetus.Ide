using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitChangesTreeViewDisplay : ComponentBase
{
    /// <summary>
    /// Awkwardly, the <see cref="GitTreeViewKeyboardEventHandler"/> constructor needs this,
    /// meanwhile this component is receiving <see cref="States.GitState"/> as a cascading parameter.
    /// This should be written differently (2024-05-02).
    /// </summary>
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private GitTreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private GitTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        CommonApi.AppOptionApi.GetAppOptionsState().Options.IconSizeInPixels * (2.0 / 3.0));

    protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new GitTreeViewKeyboardEventHandler(
            CommonApi,
			GitBackgroundTaskApi.Git);

        _treeViewMouseEventHandler = new GitTreeViewMouseEventHandler(
            CommonApi,
			GitBackgroundTaskApi.Git);

        base.OnInitialized();
    }

    private Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
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
			},
			restoreFocusOnClose: null);

        CommonApi.DropdownApi.ReduceRegisterAction(dropdownRecord);
		return Task.CompletedTask;
	}
}