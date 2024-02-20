using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Displays;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

public class TreeViewSpinnerTests
{
	public TreeViewSpinner(
            Guid guid,
			ILuthetusCommonComponentRenderers commonComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(guid, isExpandable, isExpanded)
    {
		CommonComponentRenderers = commonComponentRenderers;
    }

	public ILuthetusCommonComponentRenderers CommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewSpinner treeViewSpinner)
            return false;

        return treeViewSpinner.Item == Item;
    }

    public override int GetHashCode() => Item.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            typeof(TreeViewSpinnerDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewSpinnerDisplay.TreeViewSpinner),
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
        return;
    }
}
