using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// To implement custom MouseEvent handling logic one should
/// inherit <see cref="TreeViewMouseEventHandler"/> and override the corresponding method.
/// </summary>
public class TreeViewMouseEventHandler
{
    protected readonly ITreeViewService TreeViewService;
    protected readonly BackgroundTaskService BackgroundTaskService;

    public TreeViewMouseEventHandler(
		ITreeViewService treeViewService,
		BackgroundTaskService backgroundTaskService)
    {
        TreeViewService = treeViewService;
		BackgroundTaskService = backgroundTaskService;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the async UI event handler for 'onclick' events.<br/><br/>
    /// 
    /// The synchronous version: '<see cref="OnClick(TreeViewCommandArgs)"/>' will be invoked
    /// immediately from within this method, to allow the synchronous code to block the UI purposefully.
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code.<br/><br/>
    /// </summary>
    public virtual Task OnClickAsync(TreeViewCommandArgs commandArgs)
    {
        // Run the synchronous code first to maintain the UI's synchronization context
        OnClick(commandArgs);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the async UI event handler for 'ondblclick' events.<br/><br/>
    /// 
    /// The synchronous version: '<see cref="OnDoubleClick(TreeViewCommandArgs)"/>' will be invoked
    /// immediately from within this method, to allow the synchronous code to block the UI purposefully.
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code.<br/><br/>
    /// </summary>
    public virtual Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
    {
        // Run the synchronous code first
        OnDoubleClick(commandArgs);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the async UI event handler for 'onmousedown' events.<br/><br/>
    /// 
    /// The synchronous version: '<see cref="OnMouseDown(TreeViewCommandArgs)"/>' will be invoked
    /// immediately from within this method, to allow the synchronous code to block the UI purposefully.<br/><br/>
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code.<br/><br/>
    /// </summary>
    public virtual Task OnMouseDownAsync(TreeViewCommandArgs commandArgs)
    {
        // Run the synchronous code first
        OnMouseDown(commandArgs);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the synchronous UI event handler for 'onclick' events.<br/><br/>
    /// 
    /// This method is invoked by the async version: '<see cref="OnMouseDownAsync(TreeViewCommandArgs)"/>'.<br/><br/>
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code,
    /// but for this method it makes no difference if one puts it after their code.<br/><br/>
    /// </summary>
    protected virtual void OnClick(TreeViewCommandArgs commandArgs)
    {
        return;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the synchronous UI event handler for 'ondblclick' events.<br/><br/>
    ///
    /// This method is invoked by the async version: '<see cref="OnDoubleClickAsync(TreeViewCommandArgs)"/>'.<br/><br/>
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code,
    /// but for this method it makes no difference if one puts it after their code.<br/><br/>
    /// </summary>
    protected virtual void OnDoubleClick(TreeViewCommandArgs commandArgs)
    {
        return;
    }

    /// <summary>
    /// Invoked, and awaited, as part of the synchronous UI event handler for 'onmousedown' events.<br/><br/>
    /// 
    /// This method is invoked by the async version: '<see cref="OnMouseDownAsync(TreeViewCommandArgs)"/>'.<br/><br/>
    /// 
    /// Any overrides of this method are intended to have 'base.MethodBeingOverridden()' prior to their code.<br/><br/>
    /// </summary>
    protected virtual void OnMouseDown(TreeViewCommandArgs commandArgs)
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

            TreeViewService.ReduceSetActiveNodeAction(
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
                    TreeViewService.ReduceSetActiveNodeAction(
                        commandArgs.TreeViewContainer.Key,
                        commandArgs.NodeThatReceivedMouseEvent,
                        false,
                        false);
                }
            }
        }
    }
}