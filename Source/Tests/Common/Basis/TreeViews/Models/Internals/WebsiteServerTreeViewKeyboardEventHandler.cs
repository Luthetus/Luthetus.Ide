using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

public class WebsiteServerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    public WebsiteServerTreeViewKeyboardEventHandler(
        ITreeViewService treeViewService,
        IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
    }

    public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {

        return base.OnKeyDownAsync(commandArgs);
    }
}
