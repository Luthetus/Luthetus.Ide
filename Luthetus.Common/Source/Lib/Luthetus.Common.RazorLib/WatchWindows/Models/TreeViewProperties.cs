using System.Reflection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewProperties : TreeViewWithType<WatchWindowObjectWrap>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewProperties(
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

    public override Task LoadChildBagAsync()
    {
        var oldChildrenMap = ChildBag.ToDictionary(child => child);

        try
        {
            ChildBag.Clear();

            var propertyInfoBag = Item.DebugObjectItemType.GetProperties(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static);

            foreach (var propertyInfo in propertyInfoBag)
            {
                try
                {
                    var childValue = Item.DebugObjectItem is null
                        ? null
                        : propertyInfo.GetValue(Item.DebugObjectItem);

                    var childType = propertyInfo.PropertyType;

                    // https://stackoverflow.com/questions/3762456/how-to-check-if-property-setter-is-public
                    // The getter exists and is public.
                    var hasPublicGetter = propertyInfo.CanRead &&
                        (propertyInfo.GetGetMethod( /*nonPublic*/ true)?.IsPublic ?? false);

                    var childNode = new WatchWindowObjectWrap(
                        childValue,
                        childType,
                        propertyInfo.Name,
                        hasPublicGetter);

                    ChildBag.Add(new TreeViewReflection(
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

            if (ChildBag.Count == 0)
            {
                ChildBag.Add(new TreeViewText(
                    "No properties exist for this Type",
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