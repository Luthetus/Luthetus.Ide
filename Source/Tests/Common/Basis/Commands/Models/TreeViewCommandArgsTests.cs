using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="TreeViewCommandArgs"/>
/// </summary>
public class TreeViewCommandArgsTests
{
    /// <summary>
    /// <see cref="TreeViewCommandArgs(ITreeViewService, TreeViewContainer, RazorLib.TreeViews.Models.TreeViewNoType?, Func{Task}, RazorLib.JavaScriptObjects.Models.ContextMenuFixedPosition?, Microsoft.AspNetCore.Components.Web.MouseEventArgs?, Microsoft.AspNetCore.Components.Web.KeyboardEventArgs?)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCommandArgs.TreeViewService"/>
    /// <see cref="TreeViewCommandArgs.TreeViewContainer"/>
    /// <see cref="TreeViewCommandArgs.NodeThatReceivedMouseEvent"/>
    /// <see cref="TreeViewCommandArgs.RestoreFocusToTreeView"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var services = new ServiceCollection();

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTestingSynchronous,
            LuthetusPurposeKind.Common,
            new BackgroundTaskServiceSynchronous());

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonConfig).Assembly));

        services.AddScoped(_ => hostingInformation.BackgroundTaskService);

        services.AddScoped<ITreeViewService, TreeViewService>(
            serviceProvider => new TreeViewService(
                serviceProvider.GetRequiredService<IState<TreeViewState>>(),
                serviceProvider.GetRequiredService<IBackgroundTaskService>(),
                serviceProvider.GetRequiredService<IDispatcher>()));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();
        
        var treeViewService = serviceProvider.GetRequiredService<ITreeViewService>();

        var treeViewContainerKey = Key<TreeViewContainer>.NewKey();

        var treeViewContainer = new TreeViewContainer(
            treeViewContainerKey,
            null,
            ImmutableList<TreeViewNoType>.Empty);

        treeViewService.RegisterTreeViewContainer(treeViewContainer);

        TreeViewNoType? targetNode = null;

        var treeViewCommandArgs = new TreeViewCommandArgs(
            treeViewService,
            treeViewContainer,
            targetNode,
            () => Task.CompletedTask,
            null,
            null,
            null);

        Assert.True(targetNode == treeViewCommandArgs.NodeThatReceivedMouseEvent);
        Assert.NotNull(treeViewCommandArgs.RestoreFocusToTreeView);
    }
}
