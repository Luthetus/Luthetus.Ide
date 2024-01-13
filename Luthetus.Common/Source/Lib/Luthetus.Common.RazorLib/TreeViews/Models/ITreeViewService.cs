using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.States;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public interface ITreeViewService
{
    public IState<TreeViewState> TreeViewStateWrap { get; }

    /// <summary>Duplicate keys do NOT throw an exception.</summary>
    public void RegisterTreeViewContainer(TreeViewContainer container);
    public void DisposeTreeViewContainer(Key<TreeViewContainer> containerKey);
    public void SetRoot(Key<TreeViewContainer> containerKey, TreeViewNoType node);
    public bool TryGetTreeViewContainer(Key<TreeViewContainer> containerKey, out TreeViewContainer? container);
    public void ReRenderNode(Key<TreeViewContainer> containerKey, TreeViewNoType node);
    public void AddChildNode(Key<TreeViewContainer> containerKey, TreeViewNoType parent, TreeViewNoType child);

    public void SetActiveNode(
		Key<TreeViewContainer> containerKey,
		TreeViewNoType? nextActiveNode,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void MoveLeft(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void MoveDown(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void MoveUp(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void MoveRight(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void MoveHome(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public void MoveEnd(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode);

    public string GetNodeElementId(TreeViewNoType node);
    public string GetTreeContainerElementId(Key<TreeViewContainer> containerKey);
}