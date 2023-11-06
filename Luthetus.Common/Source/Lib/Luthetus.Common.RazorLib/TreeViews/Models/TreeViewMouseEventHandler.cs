using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// To implement custom MouseEvent handling logic one should
/// inherit <see cref="TreeViewMouseEventHandler"/> and override the corresponding method.
/// </summary>
public class TreeViewMouseEventHandler
{
    protected readonly ITreeViewService _treeViewService;

    public TreeViewMouseEventHandler(ITreeViewService treeViewService)
    {
        _treeViewService = treeViewService;
    }

    /// <summary>Used for handing "onclick" events within the user interface</summary>
    public virtual void OnClick(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.MouseEventArgs is not null &&
            commandArgs.MouseEventArgs.CtrlKey &&
            commandArgs.TargetNode is not null)
        {
            _treeViewService.AddSelectedNode(
                commandArgs.TreeViewState.Key,
                commandArgs.TargetNode);
        }

        return;
    }

    /// <summary>Used for handing "ondblclick" events within the user interface</summary>
    public virtual void OnDoubleClick(TreeViewCommandArgs commandArgs)
    {
        _ = Task.Run(async () => await OnDoubleClickAsync(commandArgs));
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
        if (commandArgs.TargetNode is null)
            return;

        _treeViewService.SetActiveNode(
            commandArgs.TreeViewState.Key,
            commandArgs.TargetNode);

        // Cases where one should not clear the selected nodes
        {
            // { "Ctrl" + "LeftMouseButton" } => MultiSelection;
            if (commandArgs.MouseEventArgs is not null &&
                commandArgs.MouseEventArgs.CtrlKey &&
                commandArgs.TargetNode is not null)
            {
                return;
            }

            // { "LeftMouseButton" } => ContextMenu; &&
            // TargetNode is selected
            if (commandArgs.MouseEventArgs is not null &&
                (commandArgs.MouseEventArgs.Buttons & 1) != 1 &&
                commandArgs.TargetNode is not null &&
                commandArgs.TreeViewState.SelectedNodeBag.Any(x => x.Key == commandArgs.TargetNode.Key))
            {
                // Not pressing the left mouse button
                // so assume ContextMenu is desired result.
                return;
            }
        }

        _treeViewService.ClearSelectedNodes(commandArgs.TreeViewState.Key);
    }
}