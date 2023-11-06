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
    public virtual void OnClick(TreeViewCommandArgs commandParameter)
    {
        if (commandParameter.MouseEventArgs is not null &&
            commandParameter.MouseEventArgs.CtrlKey &&
            commandParameter.TargetNode is not null)
        {
            _treeViewService.AddSelectedNode(
                commandParameter.TreeViewState.Key,
                commandParameter.TargetNode);
        }

        return;
    }

    /// <summary>Used for handing "ondblclick" events within the user interface</summary>
    public virtual void OnDoubleClick(TreeViewCommandArgs commandParameter)
    {
        _ = Task.Run(async () => await OnDoubleClickAsync(commandParameter));
        return;
    }

    /// <summary>Used for handing "ondblclick" events within the user interface</summary>
    public virtual Task OnDoubleClickAsync(TreeViewCommandArgs commandParameter)
    {
        return Task.CompletedTask;
    }

    /// <summary>Used for handing "onmousedown" events within the user interface</summary>
    public virtual void OnMouseDown(TreeViewCommandArgs commandParameter)
    {
        if (commandParameter.TargetNode is null)
            return;

        _treeViewService.SetActiveNode(
            commandParameter.TreeViewState.Key,
            commandParameter.TargetNode);

        // Cases where one should not clear the selected nodes
        {
            // { "Ctrl" + "LeftMouseButton" } => MultiSelection;
            if (commandParameter.MouseEventArgs is not null &&
                commandParameter.MouseEventArgs.CtrlKey &&
                commandParameter.TargetNode is not null)
            {
                return;
            }

            // { "LeftMouseButton" } => ContextMenu; &&
            // TargetNode is selected
            if (commandParameter.MouseEventArgs is not null &&
                (commandParameter.MouseEventArgs.Buttons & 1) != 1 &&
                commandParameter.TargetNode is not null &&
                commandParameter.TreeViewState.SelectedNodeBag.Any(x => x.Key == commandParameter.TargetNode.Key))
            {
                // Not pressing the left mouse button
                // so assume ContextMenu is desired result.
                return;
            }
        }

        _treeViewService.ClearSelectedNodes(commandParameter.TreeViewState.Key);
    }
}