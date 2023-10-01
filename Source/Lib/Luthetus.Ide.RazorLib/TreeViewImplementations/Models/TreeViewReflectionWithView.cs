using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewReflectionWithView : TreeViewReflection
{
    public TreeViewReflectionWithView(
        WatchWindowObjectWrap watchWindowObjectWrap,
        bool isExpandable,
        bool isExpanded,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
        : base(
            watchWindowObjectWrap,
            isExpandable,
            isExpanded,
            luthetusCommonComponentRenderers)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        LuthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }
    public ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; }

    public override async Task LoadChildBagAsync()
    {
        var oldChildrenMap = ChildBag.ToDictionary(child => child);

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
            ChildBag.Add(new TreeViewException(
                e,
                false,
                false,
                LuthetusCommonComponentRenderers));
        }

        for (int i = 0; i < ChildBag.Count; i++)
        {
            var child = ChildBag[i];

            child.Parent = this;
            child.IndexAmongSiblings = i;
        }

        foreach (var newChild in ChildBag)
        {
            if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
            {
                newChild.IsExpanded = oldChild.IsExpanded;
                newChild.IsExpandable = oldChild.IsExpandable;
                newChild.IsHidden = oldChild.IsHidden;
                newChild.Key = oldChild.Key;
                newChild.ChildBag = oldChild.ChildBag;
            }
        }
    }
}