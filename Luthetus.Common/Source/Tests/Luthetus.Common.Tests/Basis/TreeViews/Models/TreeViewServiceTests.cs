using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models;

/// <summary>
/// <see cref="TreeViewService"/>
/// </summary>
public class TreeViewServiceTests
{
    /// <summary>
    /// <see cref="TreeViewService(IState{TreeViewState}, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="TreeViewService.TreeViewStateWrap"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeTreeViewServiceTests(out var treeViewService, out var backgroundTaskService);

        Assert.NotNull(treeViewService);
        Assert.NotNull(treeViewService.TreeViewStateWrap);
    }

    /// <summary>
    /// <see cref="TreeViewService.RegisterTreeViewContainer(TreeViewContainer)"/>
    /// </summary>
    [Fact]
    public void RegisterTreeViewState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.DisposeTreeViewContainer(RazorLib.Keys.Models.Key{TreeViewContainer})"/>
    /// </summary>
    [Fact]
    public void DisposeTreeViewState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.ReplaceTreeViewContainer(RazorLib.Keys.Models.Key{TreeViewContainer}, TreeViewContainer)"/>
    /// </summary>
    [Fact]
    public void ReplaceTreeViewState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.SetRoot(RazorLib.Keys.Models.Key{TreeViewContainer}, TreeViewNoType)"/>
    /// </summary>
    [Fact]
    public void SetRoot()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.TryGetTreeViewContainer(RazorLib.Keys.Models.Key{TreeViewContainer}, out TreeViewContainer?)"/>
    /// </summary>
    [Fact]
    public void TryGetTreeViewState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.ReRenderNode(RazorLib.Keys.Models.Key{TreeViewContainer}, TreeViewNoType)"/>
    /// </summary>
    [Fact]
    public void ReRenderNode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.AddChildNode(RazorLib.Keys.Models.Key{TreeViewContainer}, TreeViewNoType, TreeViewNoType)"/>
    /// </summary>
    [Fact]
    public void AddChildNode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.SetActiveNode(RazorLib.Keys.Models.Key{TreeViewContainer}, TreeViewNoType?)"/>
    /// </summary>
    [Fact]
    public void SetActiveNode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.AddSelectedNode(RazorLib.Keys.Models.Key{TreeViewContainer}, TreeViewNoType)"/>
    /// </summary>
    [Fact]
    public void AddSelectedNode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.RemoveSelectedNode(RazorLib.Keys.Models.Key{TreeViewContainer}, RazorLib.Keys.Models.Key{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveSelectedNode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.ClearSelectedNodes(RazorLib.Keys.Models.Key{TreeViewContainer})"/>
    /// </summary>
    [Fact]
    public void ClearSelectedNodes()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.MoveLeft(RazorLib.Keys.Models.Key{TreeViewContainer}, bool)"/>
    /// </summary>
    [Fact]
    public void MoveLeft()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.MoveDown(RazorLib.Keys.Models.Key{TreeViewContainer}, bool)"/>
    /// </summary>
    [Fact]
    public void MoveDown()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.MoveUp(RazorLib.Keys.Models.Key{TreeViewContainer}, bool)"/>
    /// </summary>
    [Fact]
    public void MoveUp()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.MoveRight(RazorLib.Keys.Models.Key{TreeViewContainer}, bool)"/>
    /// </summary>
    [Fact]
    public void MoveRight()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.MoveHome(RazorLib.Keys.Models.Key{TreeViewContainer}, bool)"/>
    /// </summary>
    [Fact]
    public void MoveHome()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.MoveEnd(RazorLib.Keys.Models.Key{TreeViewContainer}, bool)"/>
    /// </summary>
    [Fact]
    public void MoveEnd()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.GetNodeElementId(TreeViewNoType)"/>
    /// </summary>
    [Fact]
    public void GetNodeElementId()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewService.GetTreeContainerElementId(RazorLib.Keys.Models.Key{TreeViewContainer})"/>
    /// </summary>
    [Fact]
    public void GetTreeContainerElementId()
    {
        throw new NotImplementedException();
    }

    private void InitializeTreeViewServiceTests(
        out ITreeViewService treeViewService,
        out IBackgroundTaskService backgroundTaskService)
    {
        var services = new ServiceCollection()
            .AddScoped<ITreeViewService, TreeViewService>()
            .AddScoped<IBackgroundTaskService>(sp => new BackgroundTaskServiceSynchronous())
            .AddFluxor(options => options.ScanAssemblies(typeof(TreeViewState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        treeViewService = serviceProvider.GetRequiredService<ITreeViewService>();
        backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();
    }
}