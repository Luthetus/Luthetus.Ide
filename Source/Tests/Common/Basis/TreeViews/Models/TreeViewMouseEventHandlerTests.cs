using Luthetus.Common.RazorLib.TreeViews.Models;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models;

/// <summary>
/// <see cref="TreeViewMouseEventHandler"/>
/// </summary>
public class TreeViewMouseEventHandlerTests
{
    /// <summary>
    /// <see cref="TreeViewMouseEventHandler(ITreeViewService)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeTreeViewMouseEventHandlerTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer,
            out var mouseEventHandler);

        Assert.NotNull(mouseEventHandler);
    }

    /// <summary>
    /// <see cref="TreeViewMouseEventHandler.OnClick(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnClick()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewMouseEventHandler.OnDoubleClick(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnDoubleClick()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewMouseEventHandler.OnDoubleClickAsync(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnDoubleClickAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewMouseEventHandler.OnMouseDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnMouseDownAsync()
    {
        InitializeTreeViewMouseEventHandlerTests(
            out var dispatcher,
            out var commonTreeViews,
            out var commonComponentRenderers,
            out var treeViewStateWrap,
            out var treeViewService,
            out var backgroundTaskService,
            out var websiteServerState,
            out var websiteServer,
            out var websiteServerTreeView,
            out var websiteServerTreeViewContainer,
            out var mouseEventHandler);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            websiteServerTreeView.IsExpanded = true;
            await websiteServerTreeView.LoadChildListAsync();
        }

        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer.ActiveNode);

        await mouseEventHandler.OnMouseDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView.ChildList.First(),
                () => Task.CompletedTask,
                null,
                new MouseEventArgs(),
                null));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer!));

        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer.ActiveNode);
    }

    private void InitializeTreeViewMouseEventHandlerTests(
        out IDispatcher dispatcher,
        out LuthetusCommonTreeViews commonTreeViews,
        out LuthetusCommonComponentRenderers commonComponentRenderers,
        out IState<TreeViewState> treeViewStateWrap,
        out ITreeViewService treeViewService,
        out IBackgroundTaskService backgroundTaskService,
        out WebsiteServerState websiteServerState,
        out WebsiteServer websiteServer,
        out WebsiteServerTreeView websiteServerTreeView,
        out TreeViewContainer websiteServerTreeViewContainer,
        out TreeViewMouseEventHandler mouseEventHandler)
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

        mouseEventHandler = new TreeViewMouseEventHandler(
            treeViewService,
            backgroundTaskService);
    }
}