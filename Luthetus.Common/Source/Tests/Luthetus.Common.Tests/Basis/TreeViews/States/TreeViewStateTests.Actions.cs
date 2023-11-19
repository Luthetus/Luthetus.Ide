using Luthetus.Common.RazorLib.TreeViews.States;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

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

        var registerContainerAction = new TreeViewState.RegisterContainerAction(
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

        var disposeContainerAction = new TreeViewState.DisposeContainerAction(
            websiteServerTreeViewContainer.Key);

        Assert.Equal(websiteServerTreeViewContainer.Key, disposeContainerAction.ContainerKey);
    }

    /// <summary>
    /// <see cref="TreeViewState.WithRootNodeAction"/>
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

        await websiteServerTreeView.LoadChildBagAsync();

        var withRootNodeAction = new TreeViewState.WithRootNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView.ChildBag.First());

        Assert.Equal(websiteServerTreeViewContainer.Key, withRootNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView.ChildBag.First(), withRootNodeAction.Node);
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

        var tryGetContainerAction = new TreeViewState.TryGetContainerAction(
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

        var replaceContainerAction = new TreeViewState.ReplaceContainerAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeViewContainer);

        Assert.Equal(websiteServerTreeViewContainer.Key, replaceContainerAction.ContainerKey);
        Assert.Equal(websiteServerTreeViewContainer, replaceContainerAction.Container);
    }

    /// <summary>
    /// <see cref="TreeViewState.AddChildNodeAction"/>
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

        await websiteServerTreeView.LoadChildBagAsync();

        var addChildNodeAction = new TreeViewState.AddChildNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView,
            websiteServerTreeView.ChildBag.First());

        Assert.Equal(websiteServerTreeViewContainer.Key, addChildNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView, addChildNodeAction.ParentNode);
        Assert.Equal(websiteServerTreeView.ChildBag.First(), addChildNodeAction.ChildNode);
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

        var reRenderNodeAction = new TreeViewState.ReRenderNodeAction(
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

        var setActiveNodeAction = new TreeViewState.SetActiveNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView);

        Assert.Equal(websiteServerTreeViewContainer.Key, setActiveNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView, setActiveNodeAction.NextActiveNode);
    }

    /// <summary>
    /// <see cref="TreeViewState.AddSelectedNodeAction"/>
    /// </summary>
    [Fact]
    public void AddSelectedNodeAction()
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

        var addSelectedNodeAction = new TreeViewState.AddSelectedNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView);

        Assert.Equal(websiteServerTreeViewContainer.Key, addSelectedNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView, addSelectedNodeAction.SelectedNode);
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

        var removeSelectedNodeAction = new TreeViewState.RemoveSelectedNodeAction(
            websiteServerTreeViewContainer.Key,
            websiteServerTreeView.Key);

        Assert.Equal(websiteServerTreeViewContainer.Key, removeSelectedNodeAction.ContainerKey);
        Assert.Equal(websiteServerTreeView.Key, removeSelectedNodeAction.NodeKey);
    }

    /// <summary>
    /// <see cref="TreeViewState.ClearSelectedNodeBagAction"/>
    /// </summary>
    [Fact]
    public void ClearSelectedNodeBagAction()
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

        var clearSelectedNodeBagAction = new TreeViewState.ClearSelectedNodeBagAction(
            websiteServerTreeViewContainer.Key);

        Assert.Equal(websiteServerTreeViewContainer.Key, clearSelectedNodeBagAction.ContainerKey);
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
        
        bool shiftKey;

        // ShiftKey = false
        {
            shiftKey = false;

            var moveLeftAction = new TreeViewState.MoveLeftAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveLeftAction.ContainerKey);
            Assert.Equal(shiftKey, moveLeftAction.ShiftKey);
        }

        // ShiftKey = true
        {
            shiftKey = true;

            var moveLeftAction = new TreeViewState.MoveLeftAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);
            
            Assert.Equal(websiteServerTreeViewContainer.Key, moveLeftAction.ContainerKey);
            Assert.Equal(shiftKey, moveLeftAction.ShiftKey);
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

        bool shiftKey;

        // ShiftKey = false
        {
            shiftKey = false;

            var moveDownAction = new TreeViewState.MoveDownAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveDownAction.ContainerKey);
            Assert.Equal(shiftKey, moveDownAction.ShiftKey);
        }

        // ShiftKey = true
        {
            shiftKey = true;

            var moveDownAction = new TreeViewState.MoveDownAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveDownAction.ContainerKey);
            Assert.Equal(shiftKey, moveDownAction.ShiftKey);
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

        bool shiftKey;

        // ShiftKey = false
        {
            shiftKey = false;

            var moveUpAction = new TreeViewState.MoveUpAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveUpAction.ContainerKey);
            Assert.Equal(shiftKey, moveUpAction.ShiftKey);
        }

        // ShiftKey = true
        {
            shiftKey = true;

            var moveUpAction = new TreeViewState.MoveUpAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveUpAction.ContainerKey);
            Assert.Equal(shiftKey, moveUpAction.ShiftKey);
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

        bool shiftKey;
        Action<TreeViewNoType> loadChildBagAction = _ => { };

        // ShiftKey = false
        {
            shiftKey = false;

            var moveRightAction = new TreeViewState.MoveRightAction(
                websiteServerTreeViewContainer.Key,
                shiftKey,
                loadChildBagAction);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveRightAction.ContainerKey);
            Assert.Equal(shiftKey, moveRightAction.ShiftKey);
        }

        // ShiftKey = true
        {
            shiftKey = true;

            var moveRightAction = new TreeViewState.MoveRightAction(
                websiteServerTreeViewContainer.Key,
                shiftKey,
                loadChildBagAction);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveRightAction.ContainerKey);
            Assert.Equal(shiftKey, moveRightAction.ShiftKey);
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

        bool shiftKey;

        // ShiftKey = false
        {
            shiftKey = false;

            var moveHomeAction = new TreeViewState.MoveHomeAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveHomeAction.ContainerKey);
            Assert.Equal(shiftKey, moveHomeAction.ShiftKey);
        }

        // ShiftKey = true
        {
            shiftKey = true;

            var moveHomeAction = new TreeViewState.MoveHomeAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveHomeAction.ContainerKey);
            Assert.Equal(shiftKey, moveHomeAction.ShiftKey);
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

        bool shiftKey;

        // ShiftKey = false
        {
            shiftKey = false;

            var moveEndAction = new TreeViewState.MoveEndAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveEndAction.ContainerKey);
            Assert.Equal(shiftKey, moveEndAction.ShiftKey);
        }

        // ShiftKey = true
        {
            shiftKey = true;

            var moveEndAction = new TreeViewState.MoveEndAction(
                websiteServerTreeViewContainer.Key,
                shiftKey);

            Assert.Equal(websiteServerTreeViewContainer.Key, moveEndAction.ContainerKey);
            Assert.Equal(shiftKey, moveEndAction.ShiftKey);
        }
    }

    /// <summary>
    /// <see cref="TreeViewState.LoadChildBagAction"/>
    /// </summary>
    [Fact]
    public void LoadChildBagAction()
    {
        throw new NotImplementedException();
    }

    private void InitializeTreeViewStateActionsTests(
        out IDispatcher dispatcher,
        out LuthetusCommonTreeViews commonTreeViews,
        out LuthetusCommonComponentRenderers commonComponentRenderers,
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

        commonTreeViews = new LuthetusCommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        commonComponentRenderers = new LuthetusCommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
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