using System.Collections;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewEnumerable : TreeViewWithType<WatchWindowObjectWrap>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewEnumerable(
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
        if (obj is not TreeViewEnumerable treeViewEnumerable)
            return false;

        return treeViewEnumerable.Item == Item;
    }

    public override int GetHashCode()
    {
        return Item.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            _luthetusCommonComponentRenderers.LuthetusCommonTreeViews.TreeViewEnumerableRenderer,
            new Dictionary<string, object?>
            {
                {
                    nameof(TreeViewEnumerable),
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

            if (Item.DebugObjectItem is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();

                var genericArgument = GetGenericArgument(Item.DebugObjectItem.GetType());

                while (enumerator.MoveNext())
                {
                    var entry = enumerator.Current;

                    var childNode = new WatchWindowObjectWrap(
                        entry,
                        genericArgument,
                        genericArgument.Name,
                        Item.IsPubliclyReadable);

                    ChildBag.Add(new TreeViewReflection(
                        childNode,
                        true,
                        false,
                        _luthetusCommonComponentRenderers));
                }
            }
            else
            {
                throw new ApplicationException($"Unexpected failed cast to the Type {nameof(IEnumerable)}. {nameof(TreeViewEnumerable)} are to have a {nameof(Item.DebugObjectItem)} which is castable as {nameof(IEnumerable)}");
            }

            if (ChildBag.Count == 0)
            {
                ChildBag.Add(new TreeViewText(
                    "Enumeration returned no results.",
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

    // https://stackoverflow.com/questions/906499/getting-type-t-from-ienumerablet
    private static Type GetGenericArgument(Type type)
    {
        // Type is Array
        // short-circuit if you expect lots of arrays 
        if (type.IsArray)
            return type.GetElementType()!;

        // type is IEnumerable<T>;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];

        // type implements/extends IEnumerable<T>;
        var enumType = type.GetInterfaces()
            .Where(t => t.IsGenericType &&
                        t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
        return enumType ?? type;
    }
}