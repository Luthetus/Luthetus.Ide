using System.Reflection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewFields : TreeViewWithType<WatchWindowObjectWrap>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewFields(
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
            _luthetusCommonComponentRenderers.LuthetusCommonTreeViews.TreeViewFieldsRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewFields),
                    this
                },
            });
    }

    public override Task LoadChildBagAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildBag);

        try
        {
            ChildBag.Clear();

            var fieldInfoBag = Item.DebugObjectItemType.GetFields(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static);

            foreach (var fieldInfo in fieldInfoBag)
            {
                var childValue = Item.DebugObjectItem is null
                    ? null
                    : fieldInfo.GetValue(Item.DebugObjectItem);

                var childType = fieldInfo.FieldType;

                var childNode = new WatchWindowObjectWrap(
                    childValue,
                    childType,
                    fieldInfo.Name,
                    fieldInfo.IsPublic);

                ChildBag.Add(new TreeViewReflection(
                    childNode,
                    true,
                    false,
                    _luthetusCommonComponentRenderers));
            }

            if (ChildBag.Count == 0)
            {
                ChildBag.Add(new TreeViewText(
                    "No fields exist for this Type",
                    false,
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

        LinkChildren(previousChildren, ChildBag);

        return Task.CompletedTask;
    }
}