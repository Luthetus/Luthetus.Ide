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
        Assert.Equal(ImmutableList<TreeViewContainer>.Empty, treeViewState.ContainerBag);
    }
    
    /// <summary>
    /// <see cref="TreeViewState.ContainerBag"/>
    /// </summary>
    [Fact]
    public void ContainerBag()
    {
        var treeViewState = new TreeViewState();
        Assert.Equal(ImmutableList<TreeViewContainer>.Empty, treeViewState.ContainerBag);

        var treeViewContainer = new TreeViewContainer(
            Key<TreeViewContainer>.NewKey(),
            null,
            ImmutableList<TreeViewNoType>.Empty);

        var outContainerBag = treeViewState.ContainerBag.Add(treeViewContainer);
        Assert.NotEqual(ImmutableList<TreeViewContainer>.Empty, outContainerBag);

        var outTreeViewState = treeViewState with
        {
            ContainerBag = outContainerBag
        };

        Assert.Equal(outContainerBag, outTreeViewState.ContainerBag);
    }
}