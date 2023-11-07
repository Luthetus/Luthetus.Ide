using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;

namespace Luthetus.Common.Tests.Basis.ComponentRenderers.Models;

/// <summary>
/// <see cref="LuthetusCommonTreeViews"/>
/// </summary>
public class LuthetusCommonTreeViewsTests
{
    /// <summary>
    /// <see cref="LuthetusCommonTreeViews(Type, Type, Type, Type, Type, Type, Type, Type, Type)"/>
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
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewExceptionRendererType"/>
    /// </summary>
    [Fact]
    public void TreeViewExceptionRendererType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewMissingRendererFallbackType"/>
    /// </summary>
    [Fact]
    public void TreeViewMissingRendererFallbackType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewTextRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewTextRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewReflectionRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewReflectionRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewPropertiesRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewPropertiesRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewInterfaceImplementationRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewInterfaceImplementationRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewFieldsRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewFieldsRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewExceptionRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewExceptionRenderer()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonTreeViews.TreeViewEnumerableRenderer"/>
    /// </summary>
    [Fact]
    public void TreeViewEnumerableRenderer()
    {
        throw new NotImplementedException();
    }
}