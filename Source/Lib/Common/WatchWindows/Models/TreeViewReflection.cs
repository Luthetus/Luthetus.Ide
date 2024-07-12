using System.Collections;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewReflection : TreeViewWithType<WatchWindowObject>
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    public TreeViewReflection(
            WatchWindowObject watchWindowObject,
            bool isExpandable,
            bool isExpanded,
            ICommonComponentRenderers commonComponentRenderers)
        : base(watchWindowObject, isExpandable, isExpanded)
    {
        _commonComponentRenderers = commonComponentRenderers;
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
            _commonComponentRenderers.CommonTreeViews.TreeViewReflectionRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewReflection),
                    this
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildList);

        try
        {
            ChildList.Clear();

            ChildList.Add(new TreeViewFields(
                Item,
                true,
                false,
                _commonComponentRenderers));

            ChildList.Add(new TreeViewProperties(
                Item,
                true,
                false,
                _commonComponentRenderers));

            if (Item.Item is IEnumerable)
            {
                ChildList.Add(new TreeViewEnumerable(
                    Item,
                    true,
                    false,
                    _commonComponentRenderers));
            }

            if (Item.ItemType.IsInterface && Item.Item is not null)
            {
                var interfaceImplementation = new WatchWindowObject(
                    Item.Item,
                    Item.Item.GetType(),
                    "InterfaceImplementation",
                    false);

                ChildList.Add(new TreeViewInterfaceImplementation(
                    interfaceImplementation,
                    true,
                    false,
                    _commonComponentRenderers));
            }
        }
        catch (Exception e)
        {
            ChildList.Clear();

            ChildList.Add(new TreeViewException(
                e,
                false,
                false,
                _commonComponentRenderers));
        }

        LinkChildren(previousChildren, ChildList);

        return Task.CompletedTask;
    }
}