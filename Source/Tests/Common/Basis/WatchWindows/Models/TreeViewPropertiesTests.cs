using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary>
/// <see cref="TreeViewProperties"/>
/// </summary>
public class TreeViewPropertiesTests
{
    /// <summary>
    /// <see cref="TreeViewProperties(WatchWindowObject, bool, bool, ILuthetusCommonComponentRenderers)"/>
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
            johnDoe.Id,
            johnDoe.Id.GetType(),
            nameof(PersonTest.Id),
            true);

        var isExpandable = true;
        var isExpanded = false;

        var treeViewProperties = new TreeViewProperties(
            johnDoeWatchWindowObject,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(johnDoeWatchWindowObject, treeViewProperties.Item);
        Assert.Equal(isExpandable, treeViewProperties.IsExpandable);
        Assert.Equal(isExpanded, treeViewProperties.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewProperties.Equals(object?)"/>
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
            johnDoe.Id,
            johnDoe.Id.GetType(),
            nameof(PersonTest.Id),
            true);

        var johnDoeTreeViewProperties = new TreeViewProperties(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var janeDoeWatchWindowObject = new WatchWindowObject(
            janeDoe.Id,
            janeDoe.Id.GetType(),
            nameof(PersonTest.Id),
            true);

        var janeDoeTreeViewProperties = new TreeViewProperties(
            janeDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        // Compare against self
        Assert.True(johnDoeTreeViewProperties.Equals(johnDoeTreeViewProperties));

        // Compare against a different object with the same Type
        Assert.False(johnDoeTreeViewProperties.Equals(janeDoeTreeViewProperties));

        var treeViewText = new TreeViewText("Hello World!", true, false, commonComponentRenderers);

        // Compare against a different object NOT having the same Type
        Assert.False(johnDoeTreeViewProperties.Equals(treeViewText));
    }

    /// <summary>
    /// <see cref="TreeViewProperties.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeWatchWindowObject = new WatchWindowObject(
            johnDoe.Id,
            johnDoe.Id.GetType(),
            nameof(PersonTest.Id),
            true);

        var johnDoeTreeViewProperties = new TreeViewProperties(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        Assert.Equal(johnDoeWatchWindowObject.GetHashCode(), johnDoeTreeViewProperties.GetHashCode());
    }

    /// <summary>
    /// <see cref="TreeViewProperties.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeWatchWindowObject = new WatchWindowObject(
            johnDoe.Id,
            johnDoe.Id.GetType(),
            nameof(PersonTest.Id),
            true);

        var treeViewProperties = new TreeViewProperties(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var treeViewRenderer = treeViewProperties.GetTreeViewRenderer();

        Assert.Equal(
            commonComponentRenderers.LuthetusCommonTreeViews.TreeViewPropertiesRenderer,
            treeViewRenderer.DynamicComponentType);

        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);

        var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewProperties), parameter.Key);
        Assert.Equal(treeViewProperties, parameter.Value);
    }

    /// <summary>
    /// <see cref="TreeViewProperties.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }
}