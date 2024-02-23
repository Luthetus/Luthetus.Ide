using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// To implement custom MouseEvent handling logic one should
/// inherit <see cref="TreeViewMouseEventHandler"/> and override the corresponding method.
/// </summary>
public class TreeViewMouseEventHandler
{
    protected readonly ITreeViewService TreeViewService;
    protected readonly IBackgroundTaskService BackgroundTaskService;

    public TreeViewMouseEventHandler(
		ITreeViewService treeViewService,
		IBackgroundTaskService backgroundTaskService)
    {
        TreeViewService = treeViewService;
		BackgroundTaskService = backgroundTaskService;
    }

    /// <summary>Used for handing "onclick" events within the user interface</summary>
    public virtual void OnClick(TreeViewCommandArgs commandArgs)
    {
        return;
    }

    /// <summary>Used for handing "ondblclick" events within the user interface</summary>
    public virtual void OnDoubleClick(TreeViewCommandArgs commandArgs)
    {
		BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
        	"TreeView.OnKeyDown",
			async () => await OnDoubleClickAsync(commandArgs).ConfigureAwait(false));
        return;
    }

    /// <summary>Used for handing "ondblclick" events within the user interface</summary>
    public virtual Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        return Task.CompletedTask;
    }

    /// <summary>Used for handing "onmousedown" events within the user interface</summary>
    public virtual void OnMouseDown(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.NodeThatReceivedMouseEvent is null || commandArgs.MouseEventArgs is null)
            return;

        if ((commandArgs.MouseEventArgs.Buttons & 1) == 1) // Left Click
        {
            // This boolean asks: Should I ADD this TargetNode to the list of selected nodes,
            //                    OR
            //                    Should I CLEAR the list of selected nodes and make this TargetNode the only entry.
            var addSelectedNodes = commandArgs.MouseEventArgs.CtrlKey || commandArgs.MouseEventArgs.ShiftKey;

            // This boolean asks: Should I ALSO SELECT the nodes between the currentNode and the targetNode.
            var selectNodesBetweenCurrentAndNextActiveNode = commandArgs.MouseEventArgs.ShiftKey;

            TreeViewService.SetActiveNode(
                commandArgs.TreeViewContainer.Key,
                commandArgs.NodeThatReceivedMouseEvent,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);
        }
        else // Presume Right Click or Context Menu
        {
            if (commandArgs.MouseEventArgs.CtrlKey)
            {
                // Open context menu, but do not move the active node, regardless who the TargetNode is
            }
            else
            {
                var targetNodeAlreadySelected = commandArgs.TreeViewContainer.SelectedNodeList.Any(x => x.Key == commandArgs.NodeThatReceivedMouseEvent.Key);
                
                if (targetNodeAlreadySelected)
                {
                    // Open context menu, but do not move the active node, regardless who the TargetNode is
                }
                else
                {
                    // Move the active node, and open context menu
                    TreeViewService.SetActiveNode(
                        commandArgs.TreeViewContainer.Key,
                        commandArgs.NodeThatReceivedMouseEvent,
                        false,
                        false);
                }
            }
        }
    }
}