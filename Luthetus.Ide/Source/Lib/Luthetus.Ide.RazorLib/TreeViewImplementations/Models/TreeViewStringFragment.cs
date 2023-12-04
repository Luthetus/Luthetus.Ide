using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.TestExplorers;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

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

    public override async Task LoadChildBagAsync()
    {
        try
        {
            var previousChildren = new List<TreeViewNoType>(ChildBag);

            var newChildBag = Item.Map.Select(kvp => (TreeViewNoType)new TreeViewStringFragment(
				kvp.Value,
				CommonComponentRenderers,
				true,
				false)).ToList();

			for (var i = 0; i < newChildBag.Count; i++)
			{
				var child = (TreeViewStringFragment)newChildBag[i];
				await child.LoadChildBagAsync();

				if (child.ChildBag.Count == 0)
				{
					child.IsExpandable = false;
					child.IsExpanded = false;
				}
			}
	
			if (newChildBag.Count == 1)
			{
				// Merge parent and child

				var child = (TreeViewStringFragment)newChildBag.Single();

				Item.Value = $"{Item.Value}.{child.Item.Value}";
				Item.Map = child.Item.Map;

				newChildBag = child.ChildBag;
			}

			// newChildBag = Item.Map.Select(kvp => (TreeViewNoType)new TreeViewStringFragment(
			// 	kvp.Value,
			// 	CommonComponentRenderers,
			// 	true,
			// 	false)).ToList();

            ChildBag = newChildBag;
            LinkChildren(previousChildren, ChildBag);
        }
        catch (Exception exception)
        {
            ChildBag = new List<TreeViewNoType>
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