using System.Collections;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewReflection : TreeViewWithType<WatchWindowObjectWrap>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewReflection(
            WatchWindowObjectWrap watchWindowObjectWrap,
            bool isExpandable,
            bool isExpanded,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
        : base(watchWindowObjectWrap, isExpandable, isExpanded)
    {
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewReflection treeViewReflection)
            return false;

        return treeViewReflection.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _luthetusCommonComponentRenderers.LuthetusCommonTreeViews.TreeViewReflectionRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewReflection),
                    this
                },
            });
    }

    public override Task LoadChildBagAsync()
    {
        var oldChildrenMap = ChildBag.ToDictionary(child => child);

        try
        {
            ChildBag.Clear();

            ChildBag.Add(new TreeViewFields(
                Item,
                true,
                false,
                _luthetusCommonComponentRenderers));

            ChildBag.Add(new TreeViewProperties(
                Item,
                true,
                false,
                _luthetusCommonComponentRenderers));

            if (Item.DebugObjectItem is IEnumerable)
            {
                ChildBag.Add(new TreeViewEnumerable(Item,
                    true,
                    false,
                    _luthetusCommonComponentRenderers));
            }

            if (Item.DebugObjectItemType.IsInterface && Item.DebugObjectItem is not null)
            {
                var interfaceImplementation = new WatchWindowObjectWrap(
                    Item.DebugObjectItem,
                    Item.DebugObjectItem.GetType(),
                    "InterfaceImplementation",
                    false);

                ChildBag.Add(new TreeViewInterfaceImplementation(
                    interfaceImplementation,
                    true,
                    false,
                    _luthetusCommonComponentRenderers));
            }
        }
        catch (Exception e)
        {
            ChildBag.Clear();

            ChildBag.Add(new TreeViewException(
                e,
                false,
                false,
                _luthetusCommonComponentRenderers));
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

        return Task.CompletedTask;
    }
}