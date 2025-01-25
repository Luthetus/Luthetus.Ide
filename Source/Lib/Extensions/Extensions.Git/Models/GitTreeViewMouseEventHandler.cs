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

    public GitTreeViewMouseEventHandler(
            ITreeViewService treeViewService,
            IBackgroundTaskService backgroundTaskService,
            IState<GitState> gitStateWrap,
            IDispatcher dispatcher)
        : base(treeViewService, backgroundTaskService)
    {
        _gitStateWrap = gitStateWrap;
        _dispatcher = dispatcher;
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
            var widthViewportValue = dialogViewModel.DialogElementDimensions.WidthDimensionAttribute.DimensionUnitList.First(
                x => x.DimensionUnitKind == DimensionUnitKind.ViewportWidth);
            widthViewportValue.Value = 95;

            // Since the width is changed, one must update the left respectively.
            var leftViewportValue = dialogViewModel.DialogElementDimensions.LeftDimensionAttribute.DimensionUnitList.First(
                x => x.DimensionUnitKind == DimensionUnitKind.ViewportWidth);
            leftViewportValue.Value = 2.5;
        }

        // Set a large height for this dialog since it requires two editors to render side by side.
        {
            var heightViewportValue = dialogViewModel.DialogElementDimensions.HeightDimensionAttribute.DimensionUnitList.First(
                x => x.DimensionUnitKind == DimensionUnitKind.ViewportHeight);
            heightViewportValue.Value = 95;

            // Since the height is changed, one must update the top respectively.
            var topViewportValue = dialogViewModel.DialogElementDimensions.TopDimensionAttribute.DimensionUnitList.First(
                x => x.DimensionUnitKind == DimensionUnitKind.ViewportHeight);
            topViewportValue.Value = 2.5;
        }

        _dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
    }
}
