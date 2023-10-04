using Luthetus.Common.RazorLib.JavaScriptObjects.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Commands.Models;

public class TreeViewCommandParameter : ICommandParameter
{
    public TreeViewCommandParameter(
        ITreeViewService treeViewService,
        TreeViewContainer treeViewState,
        TreeViewNoType? focusNode,
        Func<Task> restoreFocusToTreeView,
        ContextMenuFixedPosition? contextMenuFixedPosition,
        MouseEventArgs? mouseEventArgs,
        KeyboardEventArgs? keyboardEventArgs)
    {
        TreeViewService = treeViewService;
        TreeViewState = treeViewState;
        TargetNode = focusNode;
        RestoreFocusToTreeView = restoreFocusToTreeView;
        ContextMenuFixedPosition = contextMenuFixedPosition;
        MouseEventArgs = mouseEventArgs;
        KeyboardEventArgs = keyboardEventArgs;
    }

    public ITreeViewService TreeViewService { get; }
    public TreeViewContainer TreeViewState { get; }
    public TreeViewNoType? TargetNode { get; }
    public Func<Task> RestoreFocusToTreeView { get; }
    public ContextMenuFixedPosition? ContextMenuFixedPosition { get; }
    public MouseEventArgs? MouseEventArgs { get; }
    public KeyboardEventArgs? KeyboardEventArgs { get; }
}
