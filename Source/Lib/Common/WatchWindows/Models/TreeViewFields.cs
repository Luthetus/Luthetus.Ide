using System.Reflection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewFields : TreeViewWithType<WatchWindowObject>
{
    private readonly ICommonComponentRenderers _commonComponentRenderers;

    public TreeViewFields(
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
        if (obj is not TreeViewFields treeViewFields)
            return false;

        return treeViewFields.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _commonComponentRenderers.CommonTreeViews.TreeViewFieldsRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewFields),
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

            var fieldInfoList = Item.ItemType.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static);

            foreach (var fieldInfo in fieldInfoList)
            {
                var childValue = Item.Item is null
                    ? null
                    : fieldInfo.GetValue(Item.Item);

                var childType = fieldInfo.FieldType;

                var childNode = new WatchWindowObject(
                    childValue,
                    childType,
                    fieldInfo.Name,
                    fieldInfo.IsPublic);

                ChildList.Add(new TreeViewReflection(
                    childNode,
                    true,
                    false,
                    _commonComponentRenderers));
            }

            if (ChildList.Count == 0)
            {
                ChildList.Add(new TreeViewText(
                    "No fields exist for this Type",
                    false,
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