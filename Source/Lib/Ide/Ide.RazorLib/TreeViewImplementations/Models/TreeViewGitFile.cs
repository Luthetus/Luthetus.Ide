using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Gits.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewGitFile : TreeViewWithType<GitFile>
{
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;

    public TreeViewGitFile(
            GitFile item,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(item, isExpandable, isExpanded)
    {
        _ideComponentRenderers = ideComponentRenderers;
    }

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
            _ideComponentRenderers.LuthetusIdeTreeViews.TreeViewGitFileRendererType,
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
