namespace Luthetus.Ide.RazorLib.ComponentRenderers.Models;

public class IdeTreeViews
{
    public IdeTreeViews(
        Type treeViewNamespacePathRendererType,
        Type treeViewAbsolutePathRendererType,
        Type treeViewGitFileRendererType,
        Type treeViewCompilerServiceRendererType)
    {
        TreeViewNamespacePathRendererType = treeViewNamespacePathRendererType;
        TreeViewAbsolutePathRendererType = treeViewAbsolutePathRendererType;
        TreeViewGitFileRendererType = treeViewGitFileRendererType;
        TreeViewCompilerServiceRendererType = treeViewCompilerServiceRendererType;
    }

    public Type TreeViewNamespacePathRendererType { get; }
    public Type TreeViewAbsolutePathRendererType { get; }
    public Type TreeViewGitFileRendererType { get; }
    public Type TreeViewCompilerServiceRendererType { get; }
}