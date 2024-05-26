using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Displays.Internals;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class TreeViewStringFragment : TreeViewWithType<StringFragment>
{
    public TreeViewStringFragment(
            StringFragment stringFragment,
			ILuthetusCommonComponentRenderers commonComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(stringFragment, isExpandable, isExpanded)
    {
		CommonComponentRenderers = commonComponentRenderers;
    }

	public ILuthetusCommonComponentRenderers CommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewStringFragment treeViewStringFragment)
            return false;

        return treeViewStringFragment.Item.Value == Item.Value;
    }

    public override int GetHashCode() => Item.Value.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewStringFragmentDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewStringFragmentDisplay.TreeViewStringFragment),
                    this
                },
            });
    }

    public override async Task LoadChildListAsync()
    {
        try
        {
            var previousChildren = new List<TreeViewNoType>(ChildList);

            var newChildList = Item.Map.Select(kvp => (TreeViewNoType)new TreeViewStringFragment(
				kvp.Value,
				CommonComponentRenderers,
				true,
				false)).ToList();

			for (var i = 0; i < newChildList.Count; i++)
			{
				var child = (TreeViewStringFragment)newChildList[i];
				await child.LoadChildListAsync().ConfigureAwait(false);

				if (child.ChildList.Count == 0)
				{
					child.IsExpandable = false;
					child.IsExpanded = false;
				}
			}
	
			if (newChildList.Count == 1)
			{
				// Merge parent and child

				var child = (TreeViewStringFragment)newChildList.Single();

				Item.Value = $"{Item.Value}.{child.Item.Value}";
				Item.Map = child.Item.Map;
				Item.IsEndpoint = child.Item.IsEndpoint;

				newChildList = child.ChildList;
			}

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
        return;
    }
}