using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using static Luthetus.Common.RazorLib.TreeViews.States.TreeViewState;

namespace Luthetus.Common.Tests.Basis.TreeViews.States;

/// <summary>
/// <see cref="TreeViewState"/>
/// </summary>
public class TreeViewStateActionsTests
{
    /// <summary>
    /// <see cref="TreeViewState.RegisterContainerAction"/>
    /// </summary>
    [Fact]
    public void RegisterContainerAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var registerContainerAction = new RegisterContainerAction(
            websiteServerTreeViewContainer);

        Assert.Equal(websiteServerTreeViewContainer, registerContainerAction.Container);
    }

    /// <summary>
    /// <see cref="TreeViewState.DisposeContainerAction"/>
    /// </summary>
    [Fact]
    public void DisposeContainerAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var disposeContainerAction = new DisposeContainerAction(
            websiteServerTreeViewContainer.Key);

        Assert.Equal(websiteServerTreeViewContainer.Key, disposeContainerAction.ContainerKey);
    }

    /// <summary>
    /// <see cref="WithRootNodeAction"/>
    /// </summary>
    [Fact]
    public async Task WithRootNodeActionAsync()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        await websiteServerTreeView.LoadChildListAsync();

        var withRootNodeAction = new WithRootNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView.ChildList.First());

        Assert.Equal(websiteServerTreeViewContainer.Key, withRootNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView.ChildList.First(), withRootNodeAction.Node);
    }

    /// <summary>
    /// <see cref="TreeViewState.TryGetContainerAction"/>
    /// </summary>
    [Fact]
    public void TryGetContainerAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var tryGetContainerAction = new TryGetContainerAction(
            websiteServerTreeViewContainer.Key);

        Assert.Equal(websiteServerTreeViewContainer.Key, tryGetContainerAction.ContainerKey);
    }

    /// <summary>
    /// <see cref="TreeViewState.ReplaceContainerAction"/>
    /// </summary>
    [Fact]
    public void ReplaceContainerAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var replaceContainerAction = new ReplaceContainerAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeViewContainer);

        Assert.Equal(websiteServerTreeViewContainer.Key, replaceContainerAction.ContainerKey);
        Assert.Equal(websiteServerTreeViewContainer, replaceContainerAction.Container);
    }

    /// <summary>
    /// <see cref="AddChildNodeAction"/>
    /// </summary>
    [Fact]
    public async Task AddChildNodeActionAsync()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        await websiteServerTreeView.LoadChildListAsync();

        var addChildNodeAction = new AddChildNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView,
            websiteServerTreeView.ChildList.First());

        Assert.Equal(websiteServerTreeViewContainer.Key, addChildNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView, addChildNodeAction.ParentNode);
        Assert.Equal(websiteServerTreeView.ChildList.First(), addChildNodeAction.ChildNode);
    }

    /// <summary>
    /// <see cref="TreeViewState.ReRenderNodeAction"/>
    /// </summary>
    [Fact]
    public void ReRenderNodeAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var reRenderNodeAction = new ReRenderNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView);

        Assert.Equal(websiteServerTreeViewContainer.Key, reRenderNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView, reRenderNodeAction.Node);
    }

    /// <summary>
    /// <see cref="TreeViewState.SetActiveNodeAction"/>
    /// </summary>
    [Fact]
    public void SetActiveNodeAction()
    {
        // Set active node, and clear selected nodes.
        {
            InitializeTreeViewStateActionsTests(
                out var dispatcher,
                out var commonTreeViews,
                out var commonComponentRenderers,
                out var treeViewStateWrap,
                out var treeViewService,
                out var backgroundTaskService,
                out var websiteServerState,
                out var websiteServer,
                out var websiteServerTreeView,
                out var websiteServerTreeViewContainer);

            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var setActiveNodeAction = new SetActiveNodeAction(
                websiteServerTreeViewContainer.Key,
                websiteServerTreeView,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, setActiveNodeAction.ContainerKey);
            Assert.Equal(websiteServerTreeView, setActiveNodeAction.NextActiveNode);
            Assert.Equal(addSelectedNodes, setActiveNodeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, setActiveNodeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
        
        // Clear active node and selected nodes.
        {
            InitializeTreeViewStateActionsTests(
                out var dispatcher,
                out var commonTreeViews,
                out var commonComponentRenderers,
                out var treeViewStateWrap,
                out var treeViewService,
                out var backgroundTaskService,
                out var websiteServerState,
                out var websiteServer,
                out var websiteServerTreeView,
                out var websiteServerTreeViewContainer);

            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var setActiveNodeAction = new SetActiveNodeAction(
                websiteServerTreeViewContainer.Key,
                null,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, setActiveNodeAction.ContainerKey);
            Assert.Null(setActiveNodeAction.NextActiveNode);
            Assert.Equal(addSelectedNodes, setActiveNodeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, setActiveNodeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // Set active node, do NOT clear selected nodes.
        {
            InitializeTreeViewStateActionsTests(
                out var dispatcher,
                out var commonTreeViews,
                out var commonComponentRenderers,
                out var treeViewStateWrap,
                out var treeViewService,
                out var backgroundTaskService,
                out var websiteServerState,
                out var websiteServer,
                out var websiteServerTreeView,
                out var websiteServerTreeViewContainer);

            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var setActiveNodeAction = new SetActiveNodeAction(
                websiteServerTreeViewContainer.Key,
                websiteServerTreeView,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, setActiveNodeAction.ContainerKey);
            Assert.Equal(websiteServerTreeView, setActiveNodeAction.NextActiveNode);
            Assert.Equal(addSelectedNodes, setActiveNodeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, setActiveNodeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
        
        // Set active node, do NOT clear selected nodes, and select between.
        {
            InitializeTreeViewStateActionsTests(
                out var dispatcher,
                out var commonTreeViews,
                out var commonComponentRenderers,
                out var treeViewStateWrap,
                out var treeViewService,
                out var backgroundTaskService,
                out var websiteServerState,
                out var websiteServer,
                out var websiteServerTreeView,
                out var websiteServerTreeViewContainer);

            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var setActiveNodeAction = new SetActiveNodeAction(
                websiteServerTreeViewContainer.Key,
                websiteServerTreeView,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, setActiveNodeAction.ContainerKey);
            Assert.Equal(websiteServerTreeView, setActiveNodeAction.NextActiveNode);
            Assert.Equal(addSelectedNodes, setActiveNodeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, setActiveNodeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // Set active node, clear selected nodes, and select between.
        //
        // What would even be the result of this? It seems nonsensical
        // to have addSelectedNodes == false; meanwhile
        // selectNodesBetweenCurrentAndNextActiveNode == true;
        {
            InitializeTreeViewStateActionsTests(
                out var dispatcher,
                out var commonTreeViews,
                out var commonComponentRenderers,
                out var treeViewStateWrap,
                out var treeViewService,
                out var backgroundTaskService,
                out var websiteServerState,
                out var websiteServer,
                out var websiteServerTreeView,
                out var websiteServerTreeViewContainer);

            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var setActiveNodeAction = new SetActiveNodeAction(
                websiteServerTreeViewContainer.Key,
                websiteServerTreeView,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, setActiveNodeAction.ContainerKey);
            Assert.Equal(websiteServerTreeView, setActiveNodeAction.NextActiveNode);
            Assert.Equal(addSelectedNodes, setActiveNodeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, setActiveNodeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.RemoveSelectedNodeAction"/>
    /// </summary>
    [Fact]
    public void RemoveSelectedNodeAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var removeSelectedNodeAction = new RemoveSelectedNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView.Key);

        Assert.Equal(websiteServerTreeViewContainer.Key, removeSelectedNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView.Key, removeSelectedNodeAction.KeyOfNodeToRemove);
    }

    /// <summary>
    /// <see cref="TreeViewState.MoveLeftAction"/>
    /// </summary>
    [Fact]
    public void MoveLeftAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var containerKey = websiteServerTreeViewContainer.Key;

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveLeftAction = new MoveLeftAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveLeftAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveLeftAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveLeftAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveLeftAction = new MoveLeftAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);
            
            Assert.Equal(websiteServerTreeViewContainer.Key, moveLeftAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveLeftAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveLeftAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
        
        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveLeftAction = new MoveLeftAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);
            
            Assert.Equal(websiteServerTreeViewContainer.Key, moveLeftAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveLeftAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveLeftAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveLeftAction = new MoveLeftAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);
            
            Assert.Equal(websiteServerTreeViewContainer.Key, moveLeftAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveLeftAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveLeftAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.MoveDownAction"/>
    /// </summary>
    [Fact]
    public void MoveDownAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var containerKey = websiteServerTreeViewContainer.Key;

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveDownAction = new MoveDownAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveDownAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveDownAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveDownAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveDownAction = new MoveDownAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveDownAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveDownAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveDownAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveDownAction = new MoveDownAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveDownAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveDownAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveDownAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveDownAction = new MoveDownAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveDownAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveDownAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveDownAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.MoveUpAction"/>
    /// </summary>
    [Fact]
    public void MoveUpAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var containerKey = websiteServerTreeViewContainer.Key;

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveUpAction = new MoveUpAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveUpAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveUpAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveUpAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }       

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveUpAction = new MoveUpAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveUpAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveUpAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveUpAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveUpAction = new MoveUpAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveUpAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveUpAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveUpAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveUpAction = new MoveUpAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveUpAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveUpAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveUpAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.MoveRightAction"/>
    /// </summary>
    [Fact]
    public void MoveRightAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var containerKey = websiteServerTreeViewContainer.Key;
        Action<TreeViewNoType> loadChildListAction = _ => { };

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveRightAction = new MoveRightAction(
                websiteServerTreeViewContainer.Key,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode,
                loadChildListAction);

            Assert.Equal(containerKey, moveRightAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveRightAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveRightAction.SelectNodesBetweenCurrentAndNextActiveNode);
            Assert.Equal(loadChildListAction, moveRightAction.LoadChildListAction);
        }

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveRightAction = new MoveRightAction(
                websiteServerTreeViewContainer.Key,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode,
                loadChildListAction);

            Assert.Equal(containerKey, moveRightAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveRightAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveRightAction.SelectNodesBetweenCurrentAndNextActiveNode);
            Assert.Equal(loadChildListAction, moveRightAction.LoadChildListAction);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveRightAction = new MoveRightAction(
                websiteServerTreeViewContainer.Key,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode,
                loadChildListAction);

            Assert.Equal(containerKey, moveRightAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveRightAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveRightAction.SelectNodesBetweenCurrentAndNextActiveNode);
            Assert.Equal(loadChildListAction, moveRightAction.LoadChildListAction);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveRightAction = new MoveRightAction(
                websiteServerTreeViewContainer.Key,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode,
                loadChildListAction);

            Assert.Equal(containerKey, moveRightAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveRightAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveRightAction.SelectNodesBetweenCurrentAndNextActiveNode);
            Assert.Equal(loadChildListAction, moveRightAction.LoadChildListAction);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.MoveHomeAction"/>
    /// </summary>
    [Fact]
    public void MoveHomeAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var containerKey = websiteServerTreeViewContainer.Key;

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveHomeAction = new MoveHomeAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveHomeAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveHomeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveHomeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveHomeAction = new MoveHomeAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveHomeAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveHomeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveHomeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveHomeAction = new MoveHomeAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveHomeAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveHomeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveHomeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveHomeAction = new MoveHomeAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveHomeAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveHomeAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveHomeAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.MoveEndAction"/>
    /// </summary>
    [Fact]
    public void MoveEndAction()
    {
        InitializeTreeViewStateActionsTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer);

        var containerKey = websiteServerTreeViewContainer.Key;

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveEndAction = new MoveEndAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveEndAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveEndAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveEndAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = false
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = false;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveEndAction = new MoveEndAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveEndAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveEndAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveEndAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = false
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = false;

            var moveEndAction = new MoveEndAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveEndAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveEndAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveEndAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }

        // addSelectedNodes = true
        // selectNodesBetweenCurrentAndNextActiveNode = true
        {
            var addSelectedNodes = true;
            var selectNodesBetweenCurrentAndNextActiveNode = true;

            var moveEndAction = new MoveEndAction(
                containerKey,
                addSelectedNodes,
                selectNodesBetweenCurrentAndNextActiveNode);

            Assert.Equal(containerKey, moveEndAction.ContainerKey);
            Assert.Equal(addSelectedNodes, moveEndAction.AddSelectedNodes);
            Assert.Equal(selectNodesBetweenCurrentAndNextActiveNode, moveEndAction.SelectNodesBetweenCurrentAndNextActiveNode);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.LoadChildListAction"/>
    /// </summary>
    [Fact]
    public void LoadChildListAction()
    {
        throw new NotImplementedException();
    }

    private void InitializeTreeViewStateActionsTests(
        out IDispatcher dispatcher,
        out CommonTreeViews commonTreeViews,
        out CommonComponentRenderers commonComponentRenderers,
        out IState<TreeViewState> treeViewStateWrap,
        out ITreeViewService treeViewService,
        out IBackgroundTaskService backgroundTaskService,
        out WebsiteServerState websiteServerState,
        out WebsiteServer websiteServer,
        out WebsiteServerTreeView websiteServerTreeView,
        out TreeViewContainer websiteServerTreeViewContainer)
    {
        var temporaryBackgroundTaskService = backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var continuousQueue = new BackgroundTaskQueue(
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(continuousQueue);

        var blockingQueue = new BackgroundTaskQueue(
            BlockingBackgroundTaskWorker.GetQueueKey(),
            BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        backgroundTaskService.RegisterQueue(blockingQueue);

        var services = new ServiceCollection()
            .AddScoped<ITreeViewService, TreeViewService>()
            .AddScoped(sp => temporaryBackgroundTaskService)
            .AddFluxor(options => options.ScanAssemblies(typeof(TreeViewState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        treeViewStateWrap = serviceProvider.GetRequiredService<IState<TreeViewState>>();
        treeViewService = serviceProvider.GetRequiredService<ITreeViewService>();
        backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();
        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        commonTreeViews = new CommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        commonComponentRenderers = new CommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(CommonProgressNotificationDisplay),
            commonTreeViews);

        websiteServerState = new WebsiteServerState();

        websiteServer = new WebsiteServer(
            "TestServer",
            new[]
            {
                "/",
                "/index/",
                "/counter/",
                "/fetchdata/",
            },
            websiteServerState);

        websiteServerTreeView = new WebsiteServerTreeView(
            websiteServer,
            true,
            false);

        websiteServerTreeViewContainer = new TreeViewContainer(
            Key<TreeViewContainer>.NewKey(),
            websiteServerTreeView,
            new TreeViewNoType[] { websiteServerTreeView }.ToImmutableList());

        treeViewService.RegisterTreeViewContainer(websiteServerTreeViewContainer);
    }
}