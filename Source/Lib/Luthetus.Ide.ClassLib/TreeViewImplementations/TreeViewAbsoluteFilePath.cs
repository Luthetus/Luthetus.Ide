using Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.ComponentRenderers;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

public class TreeViewAbsoluteFilePath : TreeViewWithType<IAbsoluteFilePath>
{
    public TreeViewAbsoluteFilePath(
        IAbsoluteFilePath absoluteFilePath,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                absoluteFilePath,
                isExpandable,
                isExpanded)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        LuthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }
    public ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewNamespacePath treeViewSolutionExplorer)
        {
            return false;
        }

        return treeViewSolutionExplorer.Item.AbsoluteFilePath
                   .GetAbsoluteFilePathString() ==
               Item.GetAbsoluteFilePathString();
    }

    public override int GetHashCode()
    {
        return Item.GetAbsoluteFilePathString().GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.TreeViewAbsoluteFilePathRendererType!,
            new Dictionary<string, object?>
            {
            {
                nameof(ITreeViewAbsoluteFilePathRendererType.TreeViewAbsoluteFilePath),
                this
            },
            });
    }

    public override async Task LoadChildrenAsync()
    {
        try
        {
            var newChildren = new List<TreeViewNoType>();

            if (Item.IsDirectory)
            {
                newChildren = await TreeViewHelper
                    .LoadChildrenForDirectoryAsync(this);
            }

            var oldChildrenMap = Children
                .ToDictionary(child => child);

            foreach (var newChild in newChildren)
            {
                if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
                {
                    newChild.IsExpanded = oldChild.IsExpanded;
                    newChild.IsExpandable = oldChild.IsExpandable;
                    newChild.IsHidden = oldChild.IsHidden;
                    newChild.TreeViewNodeKey = oldChild.TreeViewNodeKey;
                    newChild.Children = oldChild.Children;
                }
            }

            for (int i = 0; i < newChildren.Count; i++)
            {
                var newChild = newChildren[i];

                newChild.IndexAmongSiblings = i;
                newChild.Parent = this;
                newChild.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
            }

            Children = newChildren;
        }
        catch (Exception exception)
        {
            Children = new List<TreeViewNoType>
        {
            new TreeViewException(
                exception,
                false,
                false,
                LuthetusCommonComponentRenderers.WatchWindowTreeViewRenderers)
            {
                Parent = this,
                IndexAmongSiblings = 0,
            }
        };
        }

        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        // This method is meant to do nothing in this case.
    }
}