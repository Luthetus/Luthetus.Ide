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

        var johnDoeWatchWindowObject = new WatchWindowObject(
            johnDoe,
            johnDoe.GetType(),
            johnDoe.DisplayName,
            true);

        var isExpandable = true;
        var isExpanded = false;

        var treeViewReflection = new TreeViewReflection(
            johnDoeWatchWindowObject,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(johnDoeWatchWindowObject, treeViewReflection.Item);
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

        var johnDoeWatchWindowObject = new WatchWindowObject(
            johnDoe,
            johnDoe.GetType(),
            johnDoe.DisplayName,
            true);

        var johnDoeTreeViewReflection = new TreeViewReflection(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var janeDoeWatchWindowObject = new WatchWindowObject(
            janeDoe,
            janeDoe.GetType(),
            janeDoe.DisplayName,
            true);

        var janeDoeTreeViewReflection = new TreeViewReflection(
            janeDoeWatchWindowObject,
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
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var watchWindowObject = new WatchWindowObject(
            johnDoe,
            johnDoe.GetType(),
            johnDoe.DisplayName,
            true);

        var treeViewReflection = new TreeViewReflection(
            watchWindowObject,
            true,
            false,
            commonComponentRenderers);

        Assert.Equal(watchWindowObject.GetHashCode(), treeViewReflection.GetHashCode());
    }

    /// <summary>
    /// <see cref="TreeViewReflection.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var watchWindowObject = new WatchWindowObject(
            johnDoe,
            johnDoe.GetType(),
            johnDoe.DisplayName,
            true);

        var treeViewReflection = new TreeViewReflection(
            watchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var treeViewRenderer = treeViewReflection.GetTreeViewRenderer();

        Assert.Equal(
            commonComponentRenderers.LuthetusCommonTreeViews.TreeViewReflectionRenderer,
            treeViewRenderer.DynamicComponentType);

        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);

        var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewReflection), parameter.Key);
        Assert.Equal(treeViewReflection, parameter.Value);
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