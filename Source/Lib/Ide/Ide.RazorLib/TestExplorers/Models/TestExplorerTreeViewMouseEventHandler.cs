using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class TestExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IDispatcher _dispatcher;

    public TestExplorerTreeViewMouseEventHandler(
		    ICommonComponentRenderers commonComponentRenderers,
	        IDispatcher dispatcher,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }

    public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        base.OnDoubleClickAsync(commandArgs);

        if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewStringFragment treeViewStringFragment)
            return Task.CompletedTask;

        NotificationHelper.DispatchInformative(
	        nameof(TestExplorerTreeViewMouseEventHandler),
	        nameof(OnDoubleClickAsync),
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromSeconds(5));

		return Task.CompletedTask;
    }
}