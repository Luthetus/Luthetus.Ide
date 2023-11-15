using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

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
            out var sampleContainer,
            out var sampleNode);

        var keyboardEventHandler = new TreeViewKeyboardEventHandler(treeViewService);

        Assert.NotNull(keyboardEventHandler);
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDown(RazorLib.Commands.Models.TreeViewCommandArgs)"/>
    /// </summary>
    [Fact]
    public void OnKeyDown()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewKeyboardEventHandler.OnKeyDownAsync(RazorLib.Commands.Models.TreeViewCommandArgs)"/>
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
        out TreeViewContainer sampleContainer,
        out TreeViewText sampleNode)
    {
        var services = new ServiceCollection()
            .AddScoped<ITreeViewService, TreeViewService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(TreeViewState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        treeViewStateWrap = serviceProvider.GetRequiredService<IState<TreeViewState>>();
        treeViewService = serviceProvider.GetRequiredService<ITreeViewService>();
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

        sampleContainer = new TreeViewContainer(
            Key<TreeViewContainer>.NewKey(),
            null,
            ImmutableList<TreeViewNoType>.Empty);

        sampleNode = new TreeViewText("Hello World!", true, false, commonComponentRenderers);
    }
}