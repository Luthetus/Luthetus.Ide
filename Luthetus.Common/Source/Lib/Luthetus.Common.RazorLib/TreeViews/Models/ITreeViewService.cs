using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.States;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public interface ITreeViewService
{
    public IState<TreeViewState> TreeViewStateWrap { get; }

    /// <summary>Duplicate keys do NOT throw an exception.</summary>
    public void RegisterTreeViewState(TreeViewContainer treeViewState);
    public void DisposeTreeViewState(Key<TreeViewContainer> treeViewStateKey);
    public void SetRoot(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType treeViewNoType);
    public bool TryGetTreeViewState(Key<TreeViewContainer> treeViewStateKey, out TreeViewContainer? treeViewState);
    public void ReRenderNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType node);
    public void AddChildNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType parent, TreeViewNoType child);
    public void SetActiveNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType? nextActiveNode);
    public void AddSelectedNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType nodeSelection);
    public void RemoveSelectedNode(Key<TreeViewContainer> treeViewStateKey, Key<TreeViewNoType> treeViewNodeKey);
    public void ClearSelectedNodes(Key<TreeViewContainer> treeViewStateKey);
    public void MoveLeft(Key<TreeViewContainer> treeViewStateKey, bool shiftKey);
    public void MoveDown(Key<TreeViewContainer> treeViewStateKey, bool shiftKey);
    public void MoveUp(Key<TreeViewContainer> treeViewStateKey, bool shiftKey);
    public void MoveRight(Key<TreeViewContainer> treeViewStateKey, bool shiftKey);
    public void MoveHome(Key<TreeViewContainer> treeViewStateKey, bool shiftKey);
    public void MoveEnd(Key<TreeViewContainer> treeViewStateKey, bool shiftKey);
    public string GetNodeElementId(TreeViewNoType treeViewNoType);
    public string GetTreeContainerElementId(Key<TreeViewContainer> treeViewStateKey);
}