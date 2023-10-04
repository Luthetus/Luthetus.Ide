using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.States;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public class TreeViewService : ITreeViewService
{
    private readonly IDispatcher _dispatcher;

    public TreeViewService(
        bool isEnabled,
        IState<TreeViewState> treeViewStateWrap,
        IDispatcher dispatcher)
    {
        IsEnabled = isEnabled;
        TreeViewStateWrap = treeViewStateWrap;
        _dispatcher = dispatcher;
    }

    public bool IsEnabled { get; }
    public IState<TreeViewState> TreeViewStateWrap { get; }

    public void RegisterTreeViewState(TreeViewContainer treeViewState)
    {
        var registerTreeViewStateAction = new TreeViewState.RegisterContainerAction(treeViewState);
        _dispatcher.Dispatch(registerTreeViewStateAction);
    }

    public void DisposeTreeViewState(Key<TreeViewContainer> treeViewStateKey)
    {
        var disposeTreeViewStateAction = new TreeViewState.DisposeContainerAction(treeViewStateKey);
        _dispatcher.Dispatch(disposeTreeViewStateAction);
    }

    public void ReplaceTreeViewState(Key<TreeViewContainer> treeViewStateKey, TreeViewContainer treeViewState)
    {
        var replaceTreeViewStateAction = new TreeViewState.ReplaceContainerAction(
            treeViewStateKey,
            treeViewState);

        _dispatcher.Dispatch(replaceTreeViewStateAction);
    }

    public void SetRoot(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType treeViewNoType)
    {
        var withRootAction = new TreeViewState.WithRootNodeAction(treeViewStateKey, treeViewNoType);
        _dispatcher.Dispatch(withRootAction);
    }

    public bool TryGetTreeViewState(Key<TreeViewContainer> treeViewStateKey, out TreeViewContainer? treeViewState)
    {
        treeViewState = TreeViewStateWrap.Value.ContainerBag.FirstOrDefault(
            x => x.Key == treeViewStateKey);

        return treeViewState is not null;
    }

    public void ReRenderNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType node)
    {
        var replaceNodeAction = new TreeViewState.ReRenderNodeAction(treeViewStateKey, node);
        _dispatcher.Dispatch(replaceNodeAction);
    }

    public void AddChildNode(
        Key<TreeViewContainer> treeViewStateKey,
        TreeViewNoType parent,
        TreeViewNoType child)
    {
        var addChildNodeAction = new TreeViewState.AddChildNodeAction(
            treeViewStateKey,
            parent,
            child);

        _dispatcher.Dispatch(addChildNodeAction);
    }

    public void SetActiveNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType? nextActiveNode)
    {
        var setActiveNodeAction = new TreeViewState.SetActiveNodeAction(
            treeViewStateKey,
            nextActiveNode);

        _dispatcher.Dispatch(setActiveNodeAction);
    }

    public void AddSelectedNode(Key<TreeViewContainer> treeViewStateKey, TreeViewNoType nodeSelection)
    {
        var addSelectedNodeAction = new TreeViewState.AddSelectedNodeAction(
            treeViewStateKey,
            nodeSelection);

        _dispatcher.Dispatch(addSelectedNodeAction);
    }

    public void RemoveSelectedNode(Key<TreeViewContainer> treeViewStateKey, Key<TreeViewNoType> treeViewNodeKey)
    {
        var removeSelectedNodeAction = new TreeViewState.RemoveSelectedNodeAction(
            treeViewStateKey,
            treeViewNodeKey);

        _dispatcher.Dispatch(removeSelectedNodeAction);
    }

    public void ClearSelectedNodes(Key<TreeViewContainer> treeViewStateKey)
    {
        var clearSelectedNodesAction = new TreeViewState.ClearSelectedNodeBagAction(treeViewStateKey);
        _dispatcher.Dispatch(clearSelectedNodesAction);
    }

    public void MoveLeft(Key<TreeViewContainer> treeViewStateKey, bool shiftKey)
    {
        var moveActiveSelectionLeftAction = new TreeViewState.MoveLeftAction(treeViewStateKey, shiftKey);
        _dispatcher.Dispatch(moveActiveSelectionLeftAction);
    }

    public void MoveDown(Key<TreeViewContainer> treeViewStateKey, bool shiftKey)
    {
        var moveActiveSelectionDownAction = new TreeViewState.MoveDownAction(treeViewStateKey, shiftKey);
        _dispatcher.Dispatch(moveActiveSelectionDownAction);
    }

    public void MoveUp(Key<TreeViewContainer> treeViewStateKey, bool shiftKey)
    {
        var moveActiveSelectionUpAction = new TreeViewState.MoveUpAction(treeViewStateKey, shiftKey);
        _dispatcher.Dispatch(moveActiveSelectionUpAction);
    }

    public void MoveRight(Key<TreeViewContainer> treeViewStateKey, bool shiftKey)
    {
        var moveActiveSelectionRightAction = new TreeViewState.MoveRightAction(
            treeViewStateKey,
            shiftKey,
            treeViewNoType =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await treeViewNoType.LoadChildBagAsync().ConfigureAwait(false);

                        var reRenderActiveNodeAction = new TreeViewState.ReRenderNodeAction(
                            treeViewStateKey,
                            treeViewNoType);

                        _dispatcher.Dispatch(reRenderActiveNodeAction);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }, CancellationToken.None);
            });

        _dispatcher.Dispatch(moveActiveSelectionRightAction);
    }

    public void MoveHome(Key<TreeViewContainer> treeViewStateKey, bool shiftKey)
    {
        var moveActiveSelectionHomeAction = new TreeViewState.MoveHomeAction(treeViewStateKey, shiftKey);
        _dispatcher.Dispatch(moveActiveSelectionHomeAction);
    }

    public void MoveEnd(Key<TreeViewContainer> treeViewStateKey, bool shiftKey)
    {
        var moveActiveSelectionEndAction = new TreeViewState.MoveEndAction(treeViewStateKey, shiftKey);
        _dispatcher.Dispatch(moveActiveSelectionEndAction);
    }

    public string GetNodeElementId(TreeViewNoType treeViewNoType)
    {
        return $"luth_node-{treeViewNoType.Key}";
    }

    public string GetTreeContainerElementId(Key<TreeViewContainer> treeViewStateKey)
    {
        return $"luth_tree-container-{treeViewStateKey.Guid}";
    }
}