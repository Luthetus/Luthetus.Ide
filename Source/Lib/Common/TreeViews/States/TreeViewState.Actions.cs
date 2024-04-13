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
    
	public record SetActiveNodeAction(
		Key<TreeViewContainer> ContainerKey,
		TreeViewNoType? NextActiveNode,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);
	
	public record RemoveSelectedNodeAction(
		Key<TreeViewContainer> ContainerKey,
        Key<TreeViewNoType> KeyOfNodeToRemove);

    public record MoveLeftAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record MoveDownAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record MoveUpAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record MoveRightAction(
        Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode,
		Action<TreeViewNoType> LoadChildListAction);

    public record MoveHomeAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record MoveEndAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record LoadChildListAction(
		Key<TreeViewContainer> ContainerKey,
		TreeViewNoType Node);
}