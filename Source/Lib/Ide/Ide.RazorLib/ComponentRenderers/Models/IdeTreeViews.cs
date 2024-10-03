namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public class IdeTreeViews
{
    public IdeTreeViews(
        Type treeViewNamespacePathRendererType,
        Type treeViewAbsolutePathRendererType)
    {
        TreeViewNamespacePathRendererType = treeViewNamespacePathRendererType;
        TreeViewAbsolutePathRendererType = treeViewAbsolutePathRendererType;
    }

    public Type TreeViewNamespacePathRendererType { get; }
    public Type TreeViewAbsolutePathRendererType { get; }
}