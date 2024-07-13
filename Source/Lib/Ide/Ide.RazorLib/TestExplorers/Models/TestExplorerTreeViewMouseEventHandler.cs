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
        {
        	NotificationHelper.DispatchInformative(
		        nameof(TestExplorerTreeViewMouseEventHandler),
		        $"Could not open in editor because node is not type: {nameof(TreeViewStringFragment)}",
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(5));
	        
        	return Task.CompletedTask;
        }
            
        if (treeViewStringFragment.Parent is not TreeViewStringFragment parentTreeViewStringFragment)
        {
            NotificationHelper.DispatchInformative(
		        nameof(TestExplorerTreeViewMouseEventHandler),
		        $"Could not open in editor because node's parent does not seem to include a class name",
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(5));
            
            return Task.CompletedTask;
        }
        
        var className = parentTreeViewStringFragment.Item.Value.Split('.').Last();

        NotificationHelper.DispatchInformative(
	        nameof(TestExplorerTreeViewMouseEventHandler),
	        className + ".cs",
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromSeconds(5));

		return Task.CompletedTask;
    }
}