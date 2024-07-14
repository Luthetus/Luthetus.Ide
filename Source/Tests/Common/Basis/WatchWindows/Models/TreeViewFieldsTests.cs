using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary>
/// <see cref="TreeViewFields"/>
/// </summary>
public class TreeViewFieldsTests
{
    /// <summary>
    /// <see cref="TreeViewFields(WatchWindowObject, bool, bool, ILuthetusCommonComponentRenderers)"/>
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

        var treeViewFields = new TreeViewFields(
            johnDoeWatchWindowObject,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(johnDoeWatchWindowObject, treeViewFields.Item);
        Assert.Equal(isExpandable, treeViewFields.IsExpandable);
        Assert.Equal(isExpanded, treeViewFields.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewFields.Equals(object?)"/>
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

        var johnDoeTreeViewFields = new TreeViewFields(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);
        
        var janeDoeWatchWindowObject = new WatchWindowObject(
            janeDoe.Id,
            janeDoe.Id.GetType(),
            nameof(PersonTest.Id),
            true);

        var janeDoeTreeViewFields = new TreeViewFields(
            janeDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        // Compare against self
        Assert.True(johnDoeTreeViewFields.Equals(johnDoeTreeViewFields));

        // Compare against a different object with the same Type
        Assert.False(johnDoeTreeViewFields.Equals(janeDoeTreeViewFields));

        var treeViewText = new TreeViewText("Hello World!", true, false, commonComponentRenderers);

        // Compare against a different object NOT having the same Type
        Assert.False(johnDoeTreeViewFields.Equals(treeViewText));
    }

    /// <summary>
    /// <see cref="TreeViewFields.GetHashCode()"/>
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

        var johnDoeTreeViewFields = new TreeViewFields(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        Assert.Equal(johnDoeWatchWindowObject.GetHashCode(), johnDoeTreeViewFields.GetHashCode());
    }

    /// <summary>
    /// <see cref="TreeViewFields.GetTreeViewRenderer()"/>
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

        var treeViewFields = new TreeViewFields(
            johnDoeWatchWindowObject,
            true,
            false,
            commonComponentRenderers);

        var treeViewRenderer = treeViewFields.GetTreeViewRenderer();

        Assert.Equal(
            commonComponentRenderers.CommonTreeViews.TreeViewFieldsRenderer,
            treeViewRenderer.DynamicComponentType);

        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);

        var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewFields), parameter.Key);
        Assert.Equal(treeViewFields, parameter.Value);
    }

    /// <summary>
    /// <see cref="TreeViewFields.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }
}