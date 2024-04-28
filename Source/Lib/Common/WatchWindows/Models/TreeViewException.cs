using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewException : TreeViewWithType<Exception>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewException(
            Exception exception,
            bool isExpandable,
            bool isExpanded,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
        : base(exception, isExpandable, isExpanded)
    {
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewException treeViewException)
            return false;

        return treeViewException.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _luthetusCommonComponentRenderers.LuthetusCommonTreeViews.TreeViewExceptionRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewException),
                    this
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        return Task.CompletedTask;
    }
}