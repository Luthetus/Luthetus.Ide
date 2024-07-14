using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models;

/// <summary>
/// <see cref="TreeViewKeyboardEventHandler"/>
/// </summary>
public class TreeViewKeyboardEventHandlerTests
{
    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler(ITreeViewService)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        Assert.NotNull(keyboardEventHandler);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveLeft()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        Assert.Equal(websiteServerTreeViewContainer.ActiveNode, websiteServerTreeView);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));

            // Set active node to root's first child
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));
        }

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer!));
        
        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer.ActiveNode);

        // Move to parent of active node. (this results in root being the active node)
        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                Code = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            }));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer!));
        
        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer.ActiveNode);
        Assert.True(websiteServerTreeView.IsExpanded);

        // Collapse the root node
        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                Code = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
            }));
        
        Assert.False(websiteServerTreeView.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveDown()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));
        }

        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer.ActiveNode);

        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN,
                Code = KeyboardKeyFacts.MovementKeys.ARROW_DOWN,
            }));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer));

        Assert.NotNull(websiteServerTreeViewContainer);
        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer!.ActiveNode);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveUp()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));

            // Change the active node to the root node's first child.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));

            Assert.True(treeViewService.TryGetTreeViewContainer(
                websiteServerTreeViewContainer.Key,
                out websiteServerTreeViewContainer));
        }

        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer!.ActiveNode);

        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.ARROW_UP,
                Code = KeyboardKeyFacts.MovementKeys.ARROW_UP,
            }));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer));

        Assert.NotNull(websiteServerTreeViewContainer);
        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer!.ActiveNode);

        // TODO: Add testing for more complex cases
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveRight_NOT_IsExpandedAsync()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        Assert.Equal(websiteServerTreeViewContainer.ActiveNode, websiteServerTreeView);
        Assert.Empty(websiteServerTreeView.ChildList);
        Assert.True(websiteServerTreeView.IsExpandable);
        Assert.False(websiteServerTreeView.IsExpanded);

        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            }));

        Assert.Single(websiteServerTreeView.ChildList);
        Assert.Equal(websiteServerTreeViewContainer.ActiveNode, websiteServerTreeView);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveRight_IsExpandedAsync()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));
        }

        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer.ActiveNode);

        // MoveRight for the test itself
        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
            }));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer));

        Assert.NotNull(websiteServerTreeViewContainer);
        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer!.ActiveNode);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveHomeAsync()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));

            // Change the active node to the root node's first child.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));

            Assert.True(treeViewService.TryGetTreeViewContainer(
                websiteServerTreeViewContainer.Key,
                out websiteServerTreeViewContainer));
        }

        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer!.ActiveNode);

        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.HOME,
                Code = KeyboardKeyFacts.MovementKeys.HOME,
            }));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer));

        Assert.NotNull(websiteServerTreeViewContainer);
        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer!.ActiveNode);
        
        // TODO: Add testing for more complex cases
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public async Task OnKeyDown_MoveEndAsync()
    {
        InitializeTreeViewKeyboardEventHandlerTests(
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
            out var keyboardEventHandler);

        // Test Setup
        {
            // The 'InitializeTreeViewKeyboardEventHandlerTests' method provides an unexpanded
            // node. Therefore, make the node expanded.
            await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
                treeViewService,
                websiteServerTreeViewContainer,
                websiteServerTreeView,
                () => Task.CompletedTask,
                null,
                null,
                new KeyboardEventArgs
                {
                    Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                    Code = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                }));
        }

        Assert.Equal(websiteServerTreeView, websiteServerTreeViewContainer.ActiveNode);

        // Move from root node to the only child
        await keyboardEventHandler.OnKeyDownAsync(new TreeViewCommandArgs(
            treeViewService,
            websiteServerTreeViewContainer,
            websiteServerTreeView,
            () => Task.CompletedTask,
            null,
            null,
            new KeyboardEventArgs
            {
                Key = KeyboardKeyFacts.MovementKeys.END,
                Code = KeyboardKeyFacts.MovementKeys.END,
            }));

        Assert.True(treeViewService.TryGetTreeViewContainer(
            websiteServerTreeViewContainer.Key,
            out websiteServerTreeViewContainer));

        Assert.NotNull(websiteServerTreeViewContainer);
        Assert.Equal(websiteServerTreeView.ChildList.First(), websiteServerTreeViewContainer!.ActiveNode);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDownAsync(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnKeyDownAsync()
    {
        throw new NotImplementedException();
    }

    private void InitializeTreeViewKeyboardEventHandlerTests(
        out IDispatcher dispatcher,
        out CommonTreeViews commonTreeViews,
        out CommonComponentRenderers commonComponentRenderers,
        out IState<TreeViewState> treeViewStateWrap,
        out ITreeViewService treeViewService,
        out IBackgroundTaskService backgroundTaskService,
        out WebsiteServerState websiteServerState,
        out WebsiteServer websiteServer,
        out WebsiteServerTreeView websiteServerTreeView,
        out TreeViewContainer websiteServerTreeViewContainer,
        out TreeViewKeyboardEventHandler keyboardEventHandler)
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

        keyboardEventHandler = new TreeViewKeyboardEventHandler(
            treeViewService,
            backgroundTaskService);
    }
}