using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;
using Luthetus.Common.Tests.Basis.TreeViews.Models.Internals;
using Luthetus.Common.RazorLib.Commands.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

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
    public void OnKeyDown_MoveLeft()
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        
        Assert.Equal(websiteServerTreeView.ChildBag.First(), websiteServerTreeViewContainer.ActiveNode);

        // Move to parent of active node. (this results in root being the active node)
        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
    public void OnKeyDown_MoveDown()
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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

        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        Assert.Equal(websiteServerTreeView.ChildBag.First(), websiteServerTreeViewContainer!.ActiveNode);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnKeyDown_MoveUp()
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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

        Assert.Equal(websiteServerTreeView.ChildBag.First(), websiteServerTreeViewContainer!.ActiveNode);

        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
    public void OnKeyDown_MoveRight_NOT_IsExpanded()
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
        Assert.Empty(websiteServerTreeView.ChildBag);
        Assert.True(websiteServerTreeView.IsExpandable);
        Assert.False(websiteServerTreeView.IsExpanded);

        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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

        Assert.Single(websiteServerTreeView.ChildBag);
        Assert.Equal(websiteServerTreeViewContainer.ActiveNode, websiteServerTreeView);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnKeyDown_MoveRight_IsExpanded()
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        Assert.Equal(websiteServerTreeView.ChildBag.First(), websiteServerTreeViewContainer!.ActiveNode);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnKeyDown_MoveHome()
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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

        Assert.Equal(websiteServerTreeView.ChildBag.First(), websiteServerTreeViewContainer!.ActiveNode);

        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
    public void OnKeyDown_MoveEnd()
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
            keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        keyboardEventHandler.OnKeyDown(new TreeViewCommandArgs(
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
        Assert.Equal(websiteServerTreeView.ChildBag.First(), websiteServerTreeViewContainer!.ActiveNode);
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
        out LuthetusCommonTreeViews commonTreeViews,
        out LuthetusCommonComponentRenderers commonComponentRenderers,
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
        temporaryBackgroundTaskService.RegisterQueue(ContinuousBackgroundTaskWorker.Queue);
        temporaryBackgroundTaskService.RegisterQueue(BlockingBackgroundTaskWorker.Queue);

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

        keyboardEventHandler = new TreeViewKeyboardEventHandler(treeViewService);
    }
}