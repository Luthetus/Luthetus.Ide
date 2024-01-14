using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Displays;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// <see cref="TreeViewAdhoc"/> is used when the consumer of the component does not want to show the root.<br/><br/>
/// The TreeViews were designed with a root consisting of 1 node. To get around this <see cref="TreeViewAdhoc"/>
/// can be used to have that top level root node be invisible to the user.
/// </summary>
public class TreeViewAdhoc : TreeViewWithType<byte>
{
    public TreeViewAdhoc(byte value)
        : base(value, false, true)
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewAdhoc treeViewAdhoc)
            return false;

        return treeViewAdhoc.Key == Key;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public static TreeViewAdhoc ConstructTreeViewAdhoc()
    {
        return new TreeViewAdhoc(byte.MinValue)
        {
            Parent = null,
            IsExpandable = false,
            IsExpanded = true,
            IndexAmongSiblings = 0,
            IsRoot = true,
            IsHidden = true,
            TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
        };
    }

    public static TreeViewAdhoc ConstructTreeViewAdhoc(params TreeViewNoType[] treeViewList)
    {
        var treeViewAdhoc = ConstructTreeViewAdhoc();

        var childList = treeViewList.ToList();

        for (int i = 0; i < childList.Count; i++)
        {
            var child = childList[i];

            child.IndexAmongSiblings = i;
            child.Parent = treeViewAdhoc;
        }

        treeViewAdhoc.ChildList = childList;

        return treeViewAdhoc;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewAdhocDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewAdhocDisplay.TreeViewNoTypeAdhoc),
                    this
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        // This method is meant to do nothing in this case.
        return;
    }
}