using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

namespace Luthetus.Common.RazorLib.TreeViews.Models.Utils;

public class TreeViewSpinner : TreeViewWithType<Guid>
{
    public TreeViewSpinner(
            Guid guid,
            ICommonComponentRenderers commonComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(guid, isExpandable, isExpanded)
    {
        CommonComponentRenderers = commonComponentRenderers;
    }

    public ICommonComponentRenderers CommonComponentRenderers { get; }

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
