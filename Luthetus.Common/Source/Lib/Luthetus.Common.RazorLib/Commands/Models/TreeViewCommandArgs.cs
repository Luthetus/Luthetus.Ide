using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Commands.Models;

public class TreeViewCommandArgs : ICommandArgs
{
    public TreeViewCommandArgs(
        ITreeViewService treeViewService,
        TreeViewContainer treeViewContainer,
        TreeViewNoType? targetNode,
        Func<Task> restoreFocusToTreeView,
        ContextMenuFixedPosition? contextMenuFixedPosition,
        MouseEventArgs? mouseEventArgs,
        KeyboardEventArgs? keyboardEventArgs)
    {
        TreeViewService = treeViewService;
        TreeViewContainer = treeViewContainer;
        TargetNode = targetNode;
        RestoreFocusToTreeView = restoreFocusToTreeView;
        ContextMenuFixedPosition = contextMenuFixedPosition;
        MouseEventArgs = mouseEventArgs;
        KeyboardEventArgs = keyboardEventArgs;
    }

    public ITreeViewService TreeViewService { get; }
    public TreeViewContainer TreeViewContainer { get; }
    public TreeViewNoType? TargetNode { get; }
    public Func<Task> RestoreFocusToTreeView { get; }
    public ContextMenuFixedPosition? ContextMenuFixedPosition { get; }
    public MouseEventArgs? MouseEventArgs { get; }
    public KeyboardEventArgs? KeyboardEventArgs { get; }
}
