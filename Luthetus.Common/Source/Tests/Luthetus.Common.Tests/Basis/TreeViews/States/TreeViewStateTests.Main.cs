using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.TreeViews.States;

/// <summary>
/// <see cref="TreeViewState"/>
/// </summary>
public class TreeViewStateMainTests
{
    /// <summary>
    /// <see cref="TreeViewState()"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var treeViewState = new TreeViewState();
        Assert.Equal(ImmutableList<TreeViewContainer>.Empty, treeViewState.ContainerList);
    }
    
    /// <summary>
    /// <see cref="TreeViewState.ContainerList"/>
    /// </summary>
    [Fact]
    public void ContainerList()
    {
        var treeViewState = new TreeViewState();
        Assert.Equal(ImmutableList<TreeViewContainer>.Empty, treeViewState.ContainerList);

        var treeViewContainer = new TreeViewContainer(
            Key<TreeViewContainer>.NewKey(),
            null,
            ImmutableList<TreeViewNoType>.Empty);

        var outContainerList = treeViewState.ContainerList.Add(treeViewContainer);
        Assert.NotEqual(ImmutableList<TreeViewContainer>.Empty, outContainerList);

        var outTreeViewState = treeViewState with
        {
            ContainerList = outContainerList
        };

        Assert.Equal(outContainerList, outTreeViewState.ContainerList);
    }
}