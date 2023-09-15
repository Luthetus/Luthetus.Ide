using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Common.RazorLib.WatchWindow.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

public class TreeViewCompilerService : TreeViewWithType<ICompilerService>
{
    public TreeViewCompilerService(
        ICompilerService compilerService,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers,
        bool isExpandable,
        bool isExpanded)
        : base(
            compilerService,
            isExpandable,
            isExpanded)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        LuthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }
    public ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewCompilerService treeViewCompilerService)
            return false;

        return treeViewCompilerService.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.LuthetusIdeTreeViews.TreeViewCompilerServiceRendererType!,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewCompilerServiceRendererType.TreeViewCompilerService),
                    this
                },
            });
    }

    public override Task LoadChildrenAsync()
    {
        var oldChildrenMap = Children.ToDictionary(child => child);

        try
        {
            Children.Clear();

            if (Item is CSharpCompilerService cSharpCompilerService)
            {
                Children.Add(new TreeViewCompilerService(
                    cSharpCompilerService,
                    LuthetusIdeComponentRenderers,
                    LuthetusCommonComponentRenderers,
                    true,
                    false));
            }
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

        return Task.CompletedTask;
    }
}