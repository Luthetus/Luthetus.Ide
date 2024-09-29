namespace Luthetus.Extensions.Git.ComponentRenderers.Models;

public class GitTreeViews
{
	public GitTreeViews(
		Type treeViewGitFileRendererType)
	{
		TreeViewGitFileRendererType = treeViewGitFileRendererType;
    }

    public Type TreeViewGitFileRendererType { get; }
}
