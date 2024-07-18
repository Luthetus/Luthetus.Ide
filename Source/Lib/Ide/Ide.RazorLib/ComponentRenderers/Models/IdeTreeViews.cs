namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public class IdeTreeViews
{
    public IdeTreeViews(
        Type treeViewNamespacePathRendererType,
        Type treeViewAbsolutePathRendererType,
        Type treeViewGitFileRendererType)
    {
        TreeViewNamespacePathRendererType = treeViewNamespacePathRendererType;
        TreeViewAbsolutePathRendererType = treeViewAbsolutePathRendererType;
        TreeViewGitFileRendererType = treeViewGitFileRendererType;
    }

    public Type TreeViewNamespacePathRendererType { get; }
    public Type TreeViewAbsolutePathRendererType { get; }
    public Type TreeViewGitFileRendererType { get; }
}