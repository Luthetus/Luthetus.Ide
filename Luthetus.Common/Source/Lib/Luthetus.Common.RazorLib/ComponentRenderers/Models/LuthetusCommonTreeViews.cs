namespace Luthetus.Common.RazorLib.ComponentRenderers.Models;

public class LuthetusCommonTreeViews
{
    public LuthetusCommonTreeViews(
        Type treeViewExceptionRendererType,
        Type treeViewMissingRendererFallbackType,
        Type treeViewTextRenderer,
        Type treeViewReflectionRenderer,
        Type treeViewPropertiesRenderer,
        Type treeViewInterfaceImplementationRenderer,
        Type treeViewFieldsRenderer,
        Type treeViewExceptionRenderer,
        Type treeViewEnumerableRenderer)
    {
        TreeViewExceptionRendererType = treeViewExceptionRendererType;
        TreeViewMissingRendererFallbackType = treeViewMissingRendererFallbackType;
        TreeViewTextRenderer = treeViewTextRenderer;
        TreeViewReflectionRenderer = treeViewReflectionRenderer;
        TreeViewPropertiesRenderer = treeViewPropertiesRenderer;
        TreeViewInterfaceImplementationRenderer = treeViewInterfaceImplementationRenderer;
        TreeViewFieldsRenderer = treeViewFieldsRenderer;
        TreeViewExceptionRenderer = treeViewExceptionRenderer;
        TreeViewEnumerableRenderer = treeViewEnumerableRenderer;
    }

    public Type TreeViewExceptionRendererType { get; }
    public Type TreeViewMissingRendererFallbackType { get; }
    public Type TreeViewTextRenderer { get; }
    public Type TreeViewReflectionRenderer { get; }
    public Type TreeViewPropertiesRenderer { get; }
    public Type TreeViewInterfaceImplementationRenderer { get; }
    public Type TreeViewFieldsRenderer { get; }
    public Type TreeViewExceptionRenderer { get; }
    public Type TreeViewEnumerableRenderer { get; }
}