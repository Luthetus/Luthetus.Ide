using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.States;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public class TreeViewService : ITreeViewService
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public TreeViewService(
        IState<TreeViewState> treeViewStateWrap,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        TreeViewStateWrap = treeViewStateWrap;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public IState<TreeViewState> TreeViewStateWrap { get; }

    public void RegisterTreeViewContainer(TreeViewContainer container)
    {
        var registerContainerAction = new TreeViewState.RegisterContainerAction(container);
        _dispatcher.Dispatch(registerContainerAction);
    }

    public void DisposeTreeViewContainer(Key<TreeViewContainer> containerKey)
    {
        var disposeContainerAction = new TreeViewState.DisposeContainerAction(containerKey);
        _dispatcher.Dispatch(disposeContainerAction);
    }

    public void ReplaceTreeViewContainer(Key<TreeViewContainer> containerKey, TreeViewContainer container)
    {
        var replaceContainerAction = new TreeViewState.ReplaceContainerAction(
            containerKey,
            container);

        _dispatcher.Dispatch(replaceContainerAction);
    }

    public void SetRoot(Key<TreeViewContainer> containerKey, TreeViewNoType node)
    {
        var withRootNodeAction = new TreeViewState.WithRootNodeAction(containerKey, node);
        _dispatcher.Dispatch(withRootNodeAction);
    }

    public bool TryGetTreeViewContainer(Key<TreeViewContainer> containerKey, out TreeViewContainer? container)
    {
        container = TreeViewStateWrap.Value.ContainerList.FirstOrDefault(
            x => x.Key == containerKey);

        return container is not null;
    }

    public void ReRenderNode(Key<TreeViewContainer> containerKey, TreeViewNoType node)
    {
        var reRenderNodeAction = new TreeViewState.ReRenderNodeAction(containerKey, node);
        _dispatcher.Dispatch(reRenderNodeAction);
    }

    public void AddChildNode(
        Key<TreeViewContainer> containerKey,
        TreeViewNoType parent,
        TreeViewNoType child)
    {
        var addChildNodeAction = new TreeViewState.AddChildNodeAction(
            containerKey,
            parent,
            child);

        _dispatcher.Dispatch(addChildNodeAction);
    }

    public void SetActiveNode(Key<TreeViewContainer> containerKey, TreeViewNoType? nextActiveNode)
    {
        var setActiveNodeAction = new TreeViewState.SetActiveNodeAction(
            containerKey,
            nextActiveNode);

        _dispatcher.Dispatch(setActiveNodeAction);
    }

    public void AddSelectedNode(Key<TreeViewContainer> containerKey, TreeViewNoType nodeSelection)
    {
        var addSelectedNodeAction = new TreeViewState.AddSelectedNodeAction(
            containerKey,
            nodeSelection);

        _dispatcher.Dispatch(addSelectedNodeAction);
    }

    public void RemoveSelectedNode(Key<TreeViewContainer> containerKey, Key<TreeViewNoType> nodeKey)
    {
        var removeSelectedNodeAction = new TreeViewState.RemoveSelectedNodeAction(
            containerKey,
            nodeKey);

        _dispatcher.Dispatch(removeSelectedNodeAction);
    }

    public void ClearSelectedNodes(Key<TreeViewContainer> containerKey)
    {
        var clearSelectedNodeListAction = new TreeViewState.ClearSelectedNodeListAction(containerKey);
        _dispatcher.Dispatch(clearSelectedNodeListAction);
    }

    public void MoveLeft(Key<TreeViewContainer> containerKey, bool shiftKey)
    {
        var moveLeftAction = new TreeViewState.MoveLeftAction(containerKey, shiftKey);
        _dispatcher.Dispatch(moveLeftAction);
    }

    public void MoveDown(Key<TreeViewContainer> containerKey, bool shiftKey)
    {
        var moveDownAction = new TreeViewState.MoveDownAction(containerKey, shiftKey);
        _dispatcher.Dispatch(moveDownAction);
    }

    public void MoveUp(Key<TreeViewContainer> containerKey, bool shiftKey)
    {
        var moveUpAction = new TreeViewState.MoveUpAction(containerKey, shiftKey);
        _dispatcher.Dispatch(moveUpAction);
    }

    public void MoveRight(Key<TreeViewContainer> containerKey, bool shiftKey)
    {
        var moveRightAction = new TreeViewState.MoveRightAction(
            containerKey,
            shiftKey,
            treeViewNoType =>
            {
                var backgroundTask = new BackgroundTask(
                    Key<BackgroundTask>.NewKey(),
                    ContinuousBackgroundTaskWorker.GetQueueKey(),
                    "TreeView.LoadChildListAsync()",
                    async () =>
                    {
                        try
                        {
                            await treeViewNoType.LoadChildListAsync().ConfigureAwait(false);

                            var reRenderNodeAction = new TreeViewState.ReRenderNodeAction(
                                containerKey,
                                treeViewNoType);

                            _dispatcher.Dispatch(reRenderNodeAction);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    });

                _backgroundTaskService.Enqueue(backgroundTask);
            });

        _dispatcher.Dispatch(moveRightAction);
    }

    public void MoveHome(Key<TreeViewContainer> containerKey, bool shiftKey)
    {
        var moveHomeAction = new TreeViewState.MoveHomeAction(containerKey, shiftKey);
        _dispatcher.Dispatch(moveHomeAction);
    }

    public void MoveEnd(Key<TreeViewContainer> containerKey, bool shiftKey)
    {
        var moveEndAction = new TreeViewState.MoveEndAction(containerKey, shiftKey);
        _dispatcher.Dispatch(moveEndAction);
    }

    public string GetNodeElementId(TreeViewNoType node)
    {
        return $"luth_node-{node.Key}";
    }

    public string GetTreeContainerElementId(Key<TreeViewContainer> containerKey)
    {
        return $"luth_tree-container-{containerKey.Guid}";
    }
}