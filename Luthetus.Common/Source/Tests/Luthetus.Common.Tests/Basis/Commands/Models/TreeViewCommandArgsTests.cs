using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.UnitTesting;
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
    /// <see cref="TreeViewCommandArgs(ITreeViewService, RazorLib.TreeViews.Models.TreeViewContainer, RazorLib.TreeViews.Models.TreeViewNoType?, Func{Task}, RazorLib.JavaScriptObjects.Models.ContextMenuFixedPosition?, Microsoft.AspNetCore.Components.Web.MouseEventArgs?, Microsoft.AspNetCore.Components.Web.KeyboardEventArgs?)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewCommandArgs.TreeViewService"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var services = new ServiceCollection();

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        CommonUnitTestHelper.AddLuthetusCommonServicesUnitTesting(services, hostingInformation);

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly));

        services.AddScoped<ITreeViewService, TreeViewService>(
            serviceProvider => new TreeViewService(
                serviceProvider.GetRequiredService<IState<TreeViewState>>(),
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

        var treeViewCommandArgs = new TreeViewCommandArgs(
            treeViewService,
            treeViewContainer,
            null,
            () => Task.CompletedTask,
            null,
            null,
            null);
    }

    /// <summary>
    /// <see cref="TreeViewCommandArgs.TreeViewContainer"/>
    /// </summary>
    [Fact]
    public void TreeViewContainer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCommandArgs.TargetNode"/>
    /// </summary>
    [Fact]
    public void TargetNode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCommandArgs.RestoreFocusToTreeView"/>
    /// </summary>
    [Fact]
    public void RestoreFocusToTreeView()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCommandArgs.ContextMenuFixedPosition"/>
    /// </summary>
    [Fact]
    public void ContextMenuFixedPosition()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCommandArgs.MouseEventArgs"/>
    /// </summary>
    [Fact]
    public void MouseEventArgs()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewCommandArgs.KeyboardEventArgs"/>
    /// </summary>
    [Fact]
    public void KeyboardEventArgs()
    {
        throw new NotImplementedException();
    }
}
