using System.Collections;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class TreeViewEnumerable : TreeViewWithType<WatchWindowObject>
{
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;

    public TreeViewEnumerable(
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

    public override Task LoadChildListAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildList);

        try
        {
            ChildList.Clear();

            if (Item.Item is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();

                var genericArgument = GetGenericArgument(Item.Item.GetType());

                while (enumerator.MoveNext())
                {
                    var entry = enumerator.Current;

                    var childNode = new WatchWindowObject(
                        entry,
                        genericArgument,
                        genericArgument.Name,
                        Item.IsPubliclyReadable);

                    ChildList.Add(new TreeViewReflection(
                        childNode,
                        true,
                        false,
                        _luthetusCommonComponentRenderers));
                }
            }
            else
            {
                throw new ApplicationException($"Unexpected failed cast to the Type {nameof(IEnumerable)}. {nameof(TreeViewEnumerable)} are to have a {nameof(Item.Item)} which is castable as {nameof(IEnumerable)}");
            }

            if (ChildList.Count == 0)
            {
                ChildList.Add(new TreeViewText(
                    "Enumeration returned no results.",
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