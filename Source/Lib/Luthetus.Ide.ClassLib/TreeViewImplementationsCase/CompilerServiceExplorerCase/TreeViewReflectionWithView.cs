using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewClasses;
using Luthetus.Ide.ClassLib.ComponentRenderersCase;

namespace Luthetus.Ide.ClassLib.TreeViewImplementationsCase.CompilerServiceExplorerCase;

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

    public override async Task LoadChildrenAsync()
    {
        var oldChildrenMap = Children.ToDictionary(child => child);

        Children.Clear();

        await base.LoadChildrenAsync();

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
            Children.Clear();
            Children.Add(new TreeViewException(
                e,
                false,
                false,
                LuthetusCommonComponentRenderers));
        }

        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            child.Parent = this;
            child.IndexAmongSiblings = i;
        }

        foreach (var newChild in Children)
        {
            if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
            {
                newChild.IsExpanded = oldChild.IsExpanded;
                newChild.IsExpandable = oldChild.IsExpandable;
                newChild.IsHidden = oldChild.IsHidden;
                newChild.Key = oldChild.Key;
                newChild.Children = oldChild.Children;
            }
        }
    }
}