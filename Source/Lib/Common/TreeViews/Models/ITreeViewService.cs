using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public interface ITreeViewService
{
    public event Action? TreeViewStateChanged;
    
    public TreeViewState GetTreeViewState();
    public TreeViewContainer GetTreeViewContainer(Key<TreeViewContainer> containerKey);

    public bool TryGetTreeViewContainer(Key<TreeViewContainer> containerKey, out TreeViewContainer? container);

    public void MoveRight(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public string GetNodeElementId(TreeViewNoType node);

    public string GetTreeViewContainerElementId(Key<TreeViewContainer> containerKey);
		
	// Reducer methods
    public void ReduceRegisterContainerAction(TreeViewContainer container);

    public void ReduceDisposeContainerAction(Key<TreeViewContainer> containerKey);

    public void ReduceWithRootNodeAction(Key<TreeViewContainer> containerKey, TreeViewNoType node);

    public void ReduceAddChildNodeAction(Key<TreeViewContainer> containerKey, TreeViewNoType parentNode, TreeViewNoType childNode);

    public void ReduceReRenderNodeAction(Key<TreeViewContainer> containerKey, TreeViewNoType node);

    public void ReduceSetActiveNodeAction(
    	Key<TreeViewContainer> containerKey,
		TreeViewNoType? nextActiveNode,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void ReduceRemoveSelectedNodeAction(
    	Key<TreeViewContainer> containerKey,
        Key<TreeViewNoType> keyOfNodeToRemove);

    public void ReduceMoveLeftAction(
    	Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void ReduceMoveDownAction(
    	Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void ReduceMoveUpAction(
    	Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void ReduceMoveRightAction(
        Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode,
		Action<TreeViewNoType> loadChildListAction);

    public void ReduceMoveHomeAction(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void ReduceMoveEndAction(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);
}