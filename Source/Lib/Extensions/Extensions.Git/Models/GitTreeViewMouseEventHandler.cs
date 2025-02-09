using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.Git.Displays;
using Luthetus.Extensions.Git.States;

namespace Luthetus.Extensions.Git.Models;

public class GitTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly IState<GitState> _gitStateWrap;
    private readonly IDispatcher _dispatcher;
    private readonly IDialogService _dialogService;

    public GitTreeViewMouseEventHandler(
            ITreeViewService treeViewService,
            IBackgroundTaskService backgroundTaskService,
            IState<GitState> gitStateWrap,
            IDispatcher dispatcher,
            IDialogService dialogService)
        : base(treeViewService, backgroundTaskService)
    {
        _gitStateWrap = gitStateWrap;
        _dispatcher = dispatcher;
        _dialogService = dialogService;
    }

    protected override void OnDoubleClick(TreeViewCommandArgs commandArgs)
    {
        base.OnDoubleClick(commandArgs);

        if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewGitFile treeViewGitFile)
            return;

        var dialogViewModel = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"Diff: {treeViewGitFile.Item.AbsolutePath.NameWithExtension}",
            typeof(GitDiffDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(GitDiffDisplay.GitFile),
                    treeViewGitFile.Item
                }
            },
            null,
            true,
            null);

        // Set a large width for this dialog since it requires two editors to render side by side.
        {
        	dialogViewModel.DialogElementDimensions.WidthDimensionAttribute.Set(95, DimensionUnitKind.ViewportWidth);

            // Since the width is changed, one must update the left respectively.
            dialogViewModel.DialogElementDimensions.LeftDimensionAttribute.Set(2.5, DimensionUnitKind.ViewportWidth);
        }

        // Set a large height for this dialog since it requires two editors to render side by side.
        {
        	dialogViewModel.DialogElementDimensions.HeightDimensionAttribute.Set(95, DimensionUnitKind.ViewportHeight);

            // Since the height is changed, one must update the top respectively.
            dialogViewModel.DialogElementDimensions.TopDimensionAttribute.Set(2.5, DimensionUnitKind.ViewportHeight);
        }

        _dialogService.ReduceRegisterAction(dialogViewModel);
    }
}
