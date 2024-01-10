using Luthetus.Common.RazorLib.WatchWindows.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary>
/// <see cref="TreeViewText"/>
/// </summary>
public class TreeViewTextTests
{
    /// <summary>
    /// <see cref="TreeViewText(string, bool, bool, RazorLib.ComponentRenderers.Models.ILuthetusCommonComponentRenderers)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var text = johnDoe.DisplayName;
        var isExpandable = true;
        var isExpanded = false;

        var treeViewReflection = new TreeViewText(
            text,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(text, treeViewReflection.Item);
        Assert.Equal(isExpandable, treeViewReflection.IsExpandable);
        Assert.Equal(isExpanded, treeViewReflection.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewText.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var johnDoeTreeViewText = new TreeViewText(
            johnDoe.DisplayName,
            true,
            false,
            commonComponentRenderers);

        var janeDoeTreeViewText = new TreeViewText(
            janeDoe.DisplayName,
            true,
            false,
            commonComponentRenderers);

        // Compare against self
        Assert.True(johnDoeTreeViewText.Equals(johnDoeTreeViewText));

        // Compare against a different object with the same Type
        Assert.False(johnDoeTreeViewText.Equals(janeDoeTreeViewText));

        var treeViewText = new TreeViewText("Hello World!", true, false, commonComponentRenderers);

        // Compare against a different object NOT having the same Type
        Assert.False(johnDoeTreeViewText.Equals(treeViewText));
    }

    /// <summary>
    /// <see cref="TreeViewText.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var text = johnDoe.DisplayName;

        var treeViewText = new TreeViewText(
            text,
            true,
            false,
            commonComponentRenderers);

        Assert.Equal(text.GetHashCode(), treeViewText.GetHashCode());
    }

    /// <summary>
    /// <see cref="TreeViewText.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var treeViewText = new TreeViewText(
            johnDoe.DisplayName,
            true,
            false,
            commonComponentRenderers);

        var treeViewRenderer = treeViewText.GetTreeViewRenderer();

        Assert.Equal(
            commonComponentRenderers.LuthetusCommonTreeViews.TreeViewTextRenderer,
            treeViewRenderer.DynamicComponentType);

        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);

        var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewText), parameter.Key);
        Assert.Equal(treeViewText, parameter.Value);
    }

    /// <summary>
    /// <see cref="TreeViewText.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildListAsync()
    {
        throw new NotImplementedException();
    }
}