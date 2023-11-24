using Luthetus.Common.RazorLib.WatchWindows.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary>
/// <see cref="TreeViewReflection"/>
/// </summary>
public class TreeViewReflectionTests
{
    /// <summary>
    /// <see cref="TreeViewReflection(WatchWindowObject, bool, bool, RazorLib.ComponentRenderers.Models.ILuthetusCommonComponentRenderers)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeRelativesWatchWindowObject = new WatchWindowObject(
            johnDoe,
            johnDoe.GetType(),
            nameof(PersonTest),
            true);

        var isExpandable = true;
        var isExpanded = false;

        var treeViewReflection = new TreeViewReflection(
            johnDoeRelativesWatchWindowObject,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(johnDoeRelativesWatchWindowObject, treeViewReflection.Item);
        Assert.Equal(isExpandable, treeViewReflection.IsExpandable);
        Assert.Equal(isExpanded, treeViewReflection.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewReflection.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeRelativesWatchWindowObject = new WatchWindowObject(
            johnDoe,
            johnDoe.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var johnDoeTreeViewReflection = new TreeViewReflection(
            johnDoeRelativesWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var janeDoeRelativesWatchWindowObject = new WatchWindowObject(
            janeDoe,
            janeDoe.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var janeDoeTreeViewReflection = new TreeViewReflection(
            janeDoeRelativesWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        // Compare against self
        Assert.True(johnDoeTreeViewReflection.Equals(johnDoeTreeViewReflection));

        // Compare against a different object with the same Type
        Assert.False(johnDoeTreeViewReflection.Equals(janeDoeTreeViewReflection));

        var treeViewText = new TreeViewText("Hello World!", true, false, commonComponentRenderers);

        // Compare against a different object NOT having the same Type
        Assert.False(johnDoeTreeViewReflection.Equals(treeViewText));
    }

    /// <summary>
    /// <see cref="TreeViewReflection.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewReflection.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TreeViewReflection.LoadChildBagAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildBagAsync()
    {
        throw new NotImplementedException();
    }
}