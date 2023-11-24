using Luthetus.Common.RazorLib.WatchWindows.Models;

namespace Luthetus.Common.Tests.Basis.WatchWindows.Models;

/// <summary>
/// <see cref="TreeViewException"/>
/// </summary>
public class TreeViewExceptionTests
{
    /// <summary>
    /// <see cref="TreeViewException(Exception, bool, bool, RazorLib.ComponentRenderers.Models.ILuthetusCommonComponentRenderers)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var item = new Exception("Abc123");
        var isExpandable = false;
        var isExpanded = false;

        var treeViewException = new TreeViewException(
            item,
            isExpandable,
            isExpanded,
            commonComponentRenderers);

        Assert.Equal(item, treeViewException.Item);
        Assert.Equal(isExpandable, treeViewException.IsExpandable);
        Assert.Equal(isExpanded, treeViewException.IsExpanded);
    }

    /// <summary>
    /// <see cref="TreeViewException.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var abc123Exception = new Exception("Abc123");
        var abc123ExceptionIsExpandable = false;
        var abc123ExceptionIsExpanded = false;

        var abc123ExceptionTreeViewException = new TreeViewException(
            abc123Exception,
            abc123ExceptionIsExpandable,
            abc123ExceptionIsExpanded,
            commonComponentRenderers);
        
        var helloWorldException = new Exception("Hello World!");
        var helloWorldExceptionIsExpandable = false;
        var helloWorldExceptionIsExpanded = false;

        var helloWorldExceptionTreeViewException = new TreeViewException(
            helloWorldException,
            helloWorldExceptionIsExpandable,
            helloWorldExceptionIsExpanded,
            commonComponentRenderers);

        // Compare against self
        Assert.True(abc123ExceptionTreeViewException.Equals(abc123ExceptionTreeViewException));

        // Compare against a different object with the same Type
        Assert.False(abc123ExceptionTreeViewException.Equals(helloWorldExceptionTreeViewException));

        var treeViewText = new TreeViewText("Hello World!", true, false, commonComponentRenderers);

        // Compare against a different object NOT having the same Type
        Assert.False(abc123ExceptionTreeViewException.Equals(treeViewText));
    }

    /// <summary>
    /// <see cref="TreeViewException.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);
        
        var abc123Exception = new Exception("Abc123");
        var abc123ExceptionIsExpandable = false;
        var abc123ExceptionIsExpanded = false;

        var treeViewException = new TreeViewException(
            abc123Exception,
            abc123ExceptionIsExpandable,
            abc123ExceptionIsExpanded,
            commonComponentRenderers);

        Assert.Equal(abc123Exception.GetHashCode(), treeViewException.GetHashCode());
    }

    /// <summary>
    /// <see cref="TreeViewException.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        WatchWindowsTestsHelper.InitializeWatchWindowsTests(
            out var johnDoe,
            out var janeDoe,
            out var bobSmith,
            out var commonComponentRenderers);

        var abc123Exception = new Exception("Abc123");
        var abc123ExceptionIsExpandable = false;
        var abc123ExceptionIsExpanded = false;

        var treeViewException = new TreeViewException(
            abc123Exception,
            abc123ExceptionIsExpandable,
            abc123ExceptionIsExpanded,
            commonComponentRenderers);

        var treeViewRenderer = treeViewException.GetTreeViewRenderer();

        Assert.Equal(
            commonComponentRenderers.LuthetusCommonTreeViews.TreeViewExceptionRendererType,
            treeViewRenderer.DynamicComponentType);

        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);

        var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewException), parameter.Key);
        Assert.Equal(treeViewException, parameter.Value);
    }

    /// <summary>
    /// <see cref="TreeViewException.LoadChildBagAsync()"/>
    /// </summary>
    [Fact]
    public void LoadChildBagAsync()
    {
        throw new NotImplementedException();
    }
}