using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewInterfaceImplementation : TreeViewReflection
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewInterfaceImplementation(
            WatchWindowObjectWrap watchWindowObjectWrap,
            bool isExpandable,
            bool isExpanded,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
        : base(watchWindowObjectWrap, isExpandable, isExpanded, luthetusCommonComponentRenderers)
    {
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _luthetusCommonComponentRenderers.LuthetusCommonTreeViews.TreeViewInterfaceImplementationRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewInterfaceImplementation),
                    this
                },
            });
    }
}