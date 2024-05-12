using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;

    public FolderExplorerTreeViewMouseEventHandler(
            LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
    }

    public override async Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        await base.OnDoubleClickAsync(commandArgs).ConfigureAwait(false);

        if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewAbsolutePath treeViewAbsolutePath)
            return;

        await _ideBackgroundTaskApi.Editor
            .OpenInEditor(treeViewAbsolutePath.Item, true)
            .ConfigureAwait(false);
    }
}