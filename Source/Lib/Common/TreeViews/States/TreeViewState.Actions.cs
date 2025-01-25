using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

public partial record TreeViewState
{
    public record struct RegisterContainerAction(TreeViewContainer Container);
    public record struct DisposeContainerAction(Key<TreeViewContainer> ContainerKey);
    public record struct WithRootNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
    public record struct TryGetContainerAction(Key<TreeViewContainer> ContainerKey);
    public record struct ReplaceContainerAction(Key<TreeViewContainer> ContainerKey, TreeViewContainer Container);

    public record struct AddChildNodeAction(
        Key<TreeViewContainer> ContainerKey, TreeViewNoType ParentNode, TreeViewNoType ChildNode);

    public record struct ReRenderNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
    
	public record struct SetActiveNodeAction(
		Key<TreeViewContainer> ContainerKey,
		TreeViewNoType? NextActiveNode,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);
	
	public record struct RemoveSelectedNodeAction(
		Key<TreeViewContainer> ContainerKey,
        Key<TreeViewNoType> KeyOfNodeToRemove);

    public record struct MoveLeftAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record struct MoveDownAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record struct MoveUpAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record struct MoveRightAction(
        Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode,
		Action<TreeViewNoType> LoadChildListAction);

    public record struct MoveHomeAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record struct MoveEndAction(
		Key<TreeViewContainer> ContainerKey,
		bool AddSelectedNodes,
		bool SelectNodesBetweenCurrentAndNextActiveNode);

    public record struct LoadChildListAction(
		Key<TreeViewContainer> ContainerKey,
		TreeViewNoType Node);
}