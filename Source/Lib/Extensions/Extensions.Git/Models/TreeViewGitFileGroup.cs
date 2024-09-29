using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.Git.Displays;

namespace Luthetus.Extensions.Git.Models;

public class TreeViewGitFileGroup : TreeViewWithType<string>
{
    public TreeViewGitFileGroup(
            string item,
            IIdeComponentRenderers ideComponentRenderers,
            ICommonComponentRenderers commonComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(item, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        CommonComponentRenderers = commonComponentRenderers;
    }

    public IIdeComponentRenderers IdeComponentRenderers { get; }
    public ICommonComponentRenderers CommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewGitFileGroup treeViewGitFileGroup)
            return false;

        return treeViewGitFileGroup.Item == Item;
    }

    public override int GetHashCode() => Item.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewGitFileGroupDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewGitFileGroupDisplay.TreeViewGitFileGroup),
                    this
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        return Task.CompletedTask;
    }
    
    public void SetChildList(TreeViewGitFile[] treeViewGitFileList)
    {
        try
        {
            var previousChildren = new List<TreeViewNoType>(ChildList);

            var newChildList = treeViewGitFileList.Select(x => (TreeViewNoType)x).ToList();

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
}
