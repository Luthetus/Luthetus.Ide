using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.WatchWindows.Displays;

namespace Luthetus.Common.Tests.Basis.ComponentRenderers.Models;

/// <summary>
/// <see cref="LuthetusCommonTreeViews"/>
/// </summary>
public class LuthetusCommonTreeViewsTests
{
    /// <summary>
    /// <see cref="LuthetusCommonTreeViews(Type, Type, Type, Type, Type, Type, Type, Type, Type)"/>
    /// <br/>----<br/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewExceptionRendererType"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewMissingRendererFallbackType"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewTextRenderer"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewReflectionRenderer"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewPropertiesRenderer"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewInterfaceImplementationRenderer"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewFieldsRenderer"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewExceptionRenderer"/>
    /// <see cref="LuthetusCommonTreeViews.TreeViewEnumerableRenderer"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var luthetusCommonTreeViews = new LuthetusCommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        Assert.NotNull(luthetusCommonTreeViews.TreeViewMissingRendererFallbackType);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewTextRenderer);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewReflectionRenderer);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewPropertiesRenderer);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewInterfaceImplementationRenderer);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewFieldsRenderer);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewExceptionRenderer);
        Assert.NotNull(luthetusCommonTreeViews.TreeViewEnumerableRenderer);
    }
}