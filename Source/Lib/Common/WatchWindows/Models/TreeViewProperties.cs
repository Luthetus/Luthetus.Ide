using System.Reflection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewProperties : TreeViewWithType<WatchWindowObject>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewProperties(
            WatchWindowObject watchWindowObject,
            bool isExpandable,
            bool isExpanded,
            ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
        : base(watchWindowObject, isExpandable, isExpanded)
    {
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewProperties treeViewProperties)
            return false;

        return treeViewProperties.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _luthetusCommonComponentRenderers.LuthetusCommonTreeViews.TreeViewPropertiesRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewProperties),
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

            var propertyInfoList = Item.ItemType.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static);

            foreach (var propertyInfo in propertyInfoList)
            {
                try
                {
                    var childValue = Item.Item is null
                        ? null
                        : propertyInfo.GetValue(Item.Item);

                    var childType = propertyInfo.PropertyType;

                    // https://stackoverflow.com/questions/3762456/how-to-check-if-property-setter-is-public
                    // The getter exists and is public.
                    var hasPublicGetter = propertyInfo.CanRead &&
                        (propertyInfo.GetGetMethod( /*nonPublic*/ true)?.IsPublic ?? false);

                    var childNode = new WatchWindowObject(
                        childValue,
                        childType,
                        propertyInfo.Name,
                        hasPublicGetter);

                    ChildList.Add(new TreeViewReflection(
                        childNode,
                        true,
                        false,
                        _luthetusCommonComponentRenderers));
                }
                catch (TargetParameterCountException)
                {
                    // Types: { 'string', 'ImmutableArray<TItem>' } at the minimum
                    // at throwing System.Reflection.TargetParameterCountException
                    // and it appears to be due to a propertyInfo for the generic type argument?
                    //
                    // Given the use case for code I am okay with continuing when this exception
                    // happens as it seems unrelated to the point of this class.
                }
            }

            if (ChildList.Count == 0)
            {
                ChildList.Add(new TreeViewText(
                    "No properties exist for this Type",
                    false,
                    false,
                    _luthetusCommonComponentRenderers));
            }
        }
        catch (Exception e)
        {
            ChildList.Clear();

            ChildList.Add(new TreeViewException(
                e,
                false,
                false,
                _luthetusCommonComponentRenderers));
        }

        LinkChildren(previousChildren, ChildList);

        return Task.CompletedTask;
    }
}