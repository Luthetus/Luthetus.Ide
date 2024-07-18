using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public class TreeViewGitFile : TreeViewWithType<GitFile>
{
    public TreeViewGitFile(
            GitFile item,
            IIdeComponentRenderers ideComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(item, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
    }

    public IIdeComponentRenderers IdeComponentRenderers { get; }

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
            IdeComponentRenderers.IdeTreeViews.TreeViewGitFileRendererType,
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
