using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

public partial record TreeViewState
{
    public record RegisterContainerAction(TreeViewContainer Container);
    public record DisposeContainerAction(Key<TreeViewContainer> ContainerKey);
    public record WithRootNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
    public record TryGetContainerAction(Key<TreeViewContainer> ContainerKey);
    public record ReplaceContainerAction(Key<TreeViewContainer> ContainerKey, TreeViewContainer Container);

    public record AddChildNodeAction(
        Key<TreeViewContainer> ContainerKey, TreeViewNoType ParentNode, TreeViewNoType ChildNode);

    public record ReRenderNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
    public record SetActiveNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType? NextActiveNode);
    public record AddSelectedNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType SelectedNode);
    public record RemoveSelectedNodeAction(Key<TreeViewContainer> ContainerKey, Key<TreeViewNoType> NodeKey);

    /// <summary>If a movement is performed on the TreeView without the "ShiftKey" being held.
    /// Then the selected nodes are cleared.</summary>
    public record ClearSelectedNodeBagAction(Key<TreeViewContainer> ContainerKey);
    public record MoveLeftAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
    public record MoveDownAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
    public record MoveUpAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);

    public record MoveRightAction(
        Key<TreeViewContainer> ContainerKey, bool ShiftKey, Action<TreeViewNoType> LoadChildBagAction);

    public record MoveHomeAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
    public record MoveEndAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
    public record LoadChildBagAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
}