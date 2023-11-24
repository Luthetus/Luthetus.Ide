using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewReflectionWithView : TreeViewReflection
{
    public TreeViewReflectionWithView(
            WatchWindowObject watchWindowObject,
            bool isExpandable,
            bool isExpanded,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            ILuthetusCommonComponentRenderers commonComponentRenderers)
    : base(watchWindowObject, isExpandable, isExpanded, commonComponentRenderers)
    {
        IdeComponentRenderers = ideComponentRenderers;
        CommonComponentRenderers = commonComponentRenderers;
    }

    public ILuthetusIdeComponentRenderers IdeComponentRenderers { get; }
    public ILuthetusCommonComponentRenderers CommonComponentRenderers { get; }

    public override async Task LoadChildBagAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildBag);

        ChildBag.Clear();

        await base.LoadChildBagAsync();

        try
        {
            // TODO: Had planned to make a more distilled view of the object as the reflection was a lot of information. (2023-09-08)
            //
            //if (Item.DebugObjectItem is ICompilerService compilerService)
            //{
            //    Children.Insert(0, new TreeViewCompilerService(
            //        compilerService,
            //        LuthetusIdeComponentRenderers,
            //        LuthetusCommonComponentRenderers,
            //        true,
            //        false));
            //}
        }
        catch (Exception e)
        {
            ChildBag.Clear();
            ChildBag.Add(new TreeViewException(e, false, false, CommonComponentRenderers));
        }

        LinkChildren(previousChildren, ChildBag);
    }
}