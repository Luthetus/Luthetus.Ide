using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Gits.Displays;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Reactive;

namespace Luthetus.Ide.RazorLib.Gits.Models;

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

    public override void OnDoubleClick(TreeViewCommandArgs commandArgs)
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
            true);

        _dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
    }
}
