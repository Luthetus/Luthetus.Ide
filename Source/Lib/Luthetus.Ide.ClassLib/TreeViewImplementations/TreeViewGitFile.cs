using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;
using Luthetus.Ide.ClassLib.Git;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

public class TreeViewGitFile : TreeViewWithType<GitFile>
{
    public TreeViewGitFile(
        GitFile gitFile,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        bool isExpandable,
        bool isExpanded)
            : base(
                gitFile,
                isExpandable,
                isExpanded)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewGitFile treeViewGitFile)
            return false;

        return treeViewGitFile.Item.AbsolutePath.FormattedInput ==
               Item.AbsolutePath.FormattedInput;
    }

    public override int GetHashCode()
    {
        return Item.AbsolutePath.FormattedInput.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.LuthetusIdeTreeViews.TreeViewGitFileRendererType!,
            new Dictionary<string, object?>
            {
            {
                nameof(ITreeViewGitFileRendererType.TreeViewGitFile),
                this
            },
            });
    }

    public override Task LoadChildrenAsync()
    {
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        // This method is meant to do nothing in this case.
    }
}