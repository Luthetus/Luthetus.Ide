using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
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
        if (obj is null ||
            obj is not TreeViewGitFile treeViewGitFile ||
            treeViewGitFile.Item is null ||
            Item is null)
        {
            return false;
        }

        return treeViewGitFile.Item.AbsoluteFilePath
                   .GetAbsoluteFilePathString() ==
               Item.AbsoluteFilePath.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item?.AbsoluteFilePath.GetAbsoluteFilePathString().GetHashCode() ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.TreeViewGitFileRendererType!,
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