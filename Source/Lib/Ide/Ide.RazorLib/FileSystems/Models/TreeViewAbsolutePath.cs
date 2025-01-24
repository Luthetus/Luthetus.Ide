using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class TreeViewAbsolutePath : TreeViewWithType<AbsolutePath>
{
    public TreeViewAbsolutePath(
            AbsolutePath absolutePath,
            IIdeComponentRenderers ideComponentRenderers,
            ICommonComponentRenderers commonComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(absolutePath, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        CommonComponentRenderers = commonComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public IIdeComponentRenderers IdeComponentRenderers { get; }
    public ICommonComponentRenderers CommonComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewAbsolutePath treeViewAbsolutePath)
            return false;

        return treeViewAbsolutePath.Item.Value == Item.Value;
    }

    public override int GetHashCode() => Item.Value.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.IdeTreeViews.TreeViewAbsolutePathRendererType,
            new Dictionary<string, object?>
            {
                { nameof(ITreeViewAbsolutePathRendererType.TreeViewAbsolutePath), this },
            });
    }

    public override async Task LoadChildListAsync()
    {
        try
        {
            var previousChildren = new List<TreeViewNoType>(ChildList);

            var newChildList = new List<TreeViewNoType>();

            if (Item.IsDirectory)
                newChildList = await TreeViewHelperAbsolutePathDirectory.LoadChildrenAsync(this).ConfigureAwait(false);

            ChildList = newChildList;
            LinkChildren(previousChildren, ChildList);
        }
        catch (Exception exception)
        {
            ChildList = new List<TreeViewNoType>
            {
                new TreeViewException(exception, false, false, CommonComponentRenderers)
                {
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }

        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        // This method is meant to do nothing in this case.
    }
}