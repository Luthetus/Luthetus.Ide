using Luthetus.Common.RazorLib.WatchWindows.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary>
/// <see cref="TreeViewEnumerable"/>
/// </summary>
public class TreeViewEnumerableTests
{
    /// <summary>
    /// <see cref="TreeViewEnumerable(WatchWindowObject, bool, bool, ILuthetusCommonComponentRenderers)"/>
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
            johnDoe.Relatives,
            johnDoe.Relatives.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var isExpandable = true;
        var isExpanded = false;

        var treeViewEnumberable = new TreeViewEnumerable(
            johnDoeRelativesWatchWindowObject,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(johnDoeRelativesWatchWindowObject, treeViewEnumberable.Item);
        Assert.Equal(isExpandable, treeViewEnumberable.IsExpandable);
        Assert.Equal(isExpanded, treeViewEnumberable.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewEnumerable.Equals(object?)"/>
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
            johnDoe.Relatives,
            johnDoe.Relatives.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var johnDoeRelativesTreeViewEnumberable = new TreeViewEnumerable(
            johnDoeRelativesWatchWindowObject,
            true,
            false,
            commonComponentRenderers);
        
        var janeDoeRelativesWatchWindowObject = new WatchWindowObject(
            janeDoe.Relatives,
            janeDoe.Relatives.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var janeDoeRelativesTreeViewEnumberable = new TreeViewEnumerable(
            janeDoeRelativesWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        // Compare against self
        Assert.True(johnDoeRelativesTreeViewEnumberable.Equals(johnDoeRelativesTreeViewEnumberable));

        // Compare against a different object with the same Type
        Assert.False(johnDoeRelativesTreeViewEnumberable.Equals(janeDoeRelativesTreeViewEnumberable));

        var treeViewText = new TreeViewText("Hello World!", true, false, commonComponentRenderers);

        // Compare against a different object NOT having the same Type
        Assert.False(johnDoeRelativesTreeViewEnumberable.Equals(treeViewText));
    }

    /// <summary>
    /// <see cref="TreeViewEnumerable.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeRelativesWatchWindowObject = new WatchWindowObject(
            johnDoe.Relatives,
            johnDoe.Relatives.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var treeViewEnumberable = new TreeViewEnumerable(
            johnDoeRelativesWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        Assert.Equal(johnDoeRelativesWatchWindowObject.GetHashCode(), treeViewEnumberable.GetHashCode());
    }

    /// <summary>
    /// <see cref="TreeViewEnumerable.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeRelativesWatchWindowObject = new WatchWindowObject(
            johnDoe.Relatives,
            johnDoe.Relatives.GetType(),
            nameof(PersonTest.Relatives),
            true);

        var treeViewEnumberable = new TreeViewEnumerable(
            johnDoeRelativesWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var treeViewRenderer = treeViewEnumberable.GetTreeViewRenderer();

        Assert.Equal(
            commonComponentRenderers.CommonTreeViews.TreeViewEnumerableRenderer,
            treeViewRenderer.DynamicComponentType);
        
        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);

        var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewEnumerable), parameter.Key);
        Assert.Equal(treeViewEnumberable, parameter.Value);
    }

    /// <summary>
    /// <see cref="TreeViewEnumerable.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }
}