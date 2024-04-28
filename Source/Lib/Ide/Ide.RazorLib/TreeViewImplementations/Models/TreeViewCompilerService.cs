using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewCompilerService : TreeViewWithType<ILuthCompilerService>
{
    public TreeViewCompilerService(
            ILuthCompilerService compilerService,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            ILuthetusCommonComponentRenderers commonComponentRenderers,
            bool isExpandable,
            bool isExpanded)
        : base(compilerService, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        CommonComponentRenderers = commonComponentRenderers;
    }

    public ILuthetusIdeComponentRenderers IdeComponentRenderers { get; }
    public ILuthetusCommonComponentRenderers CommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewCompilerService treeViewCompilerService)
            return false;

        return treeViewCompilerService.Item == Item;
    }

    public override int GetHashCode() => Item.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.LuthetusIdeTreeViews.TreeViewCompilerServiceRendererType,
            new Dictionary<string, object?>
            {
                { nameof(ITreeViewCompilerServiceRendererType.TreeViewCompilerService), this },
            });
    }

    public override Task LoadChildListAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildList);

        try
        {
            ChildList.Clear();

            if (Item is CSharpCompilerService cSharpCompilerService)
            {
                ChildList.Add(new TreeViewCompilerService(
                    cSharpCompilerService,
                    IdeComponentRenderers,
                    CommonComponentRenderers,
                    true,
                    false));
            }
        }
        catch (Exception e)
        {
            ChildList.Clear();
            ChildList.Add(new TreeViewException(e, false, false, CommonComponentRenderers));
        }

        LinkChildren(previousChildren, ChildList);

        return Task.CompletedTask;
    }
}