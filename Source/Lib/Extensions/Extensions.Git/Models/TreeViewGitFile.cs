using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.Git.ComponentRenderers.Models;

namespace Luthetus.Extensions.Git.Models;

public class TreeViewGitFile : TreeViewWithType<GitFile>
{
    public TreeViewGitFile(
            GitFile item,
            GitTreeViews gitTreeViews,
            bool isExpandable,
            bool isExpanded)
        : base(item, isExpandable, isExpanded)
    {
        GitTreeViews = gitTreeViews;
    }

    public GitTreeViews GitTreeViews { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewGitFile treeViewGitFile)
            return false;

        return treeViewGitFile.Item.AbsolutePath.Value ==
               Item.AbsolutePath.Value;
    }

    public override int GetHashCode() => Item.AbsolutePath.Value.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            GitTreeViews.TreeViewGitFileRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewGitFileRendererType.TreeViewGitFile),
                    this
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        return Task.CompletedTask;
    }
}
