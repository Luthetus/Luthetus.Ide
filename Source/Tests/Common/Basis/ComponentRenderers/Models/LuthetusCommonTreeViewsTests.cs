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
        var commonTreeViews = new CommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        Assert.NotNull(commonTreeViews.TreeViewMissingRendererFallbackType);
        Assert.NotNull(commonTreeViews.TreeViewTextRenderer);
        Assert.NotNull(commonTreeViews.TreeViewReflectionRenderer);
        Assert.NotNull(commonTreeViews.TreeViewPropertiesRenderer);
        Assert.NotNull(commonTreeViews.TreeViewInterfaceImplementationRenderer);
        Assert.NotNull(commonTreeViews.TreeViewFieldsRenderer);
        Assert.NotNull(commonTreeViews.TreeViewExceptionRenderer);
        Assert.NotNull(commonTreeViews.TreeViewEnumerableRenderer);
    }
}