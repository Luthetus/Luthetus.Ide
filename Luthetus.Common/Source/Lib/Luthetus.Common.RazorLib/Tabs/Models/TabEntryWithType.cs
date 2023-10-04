namespace Luthetus.Common.RazorLib.Tabs.Models;

public record TabEntryWithType<T> : TabEntryNoType
{
    public TabEntryWithType(
        T item,
        Func<TabEntryNoType, string> getDisplayNameFunc,
        Action<TabEntryNoType> onSetActiveFunc)
    {
        Item = item;
        GetDisplayNameFunc = getDisplayNameFunc;
        OnSetActiveFunc = onSetActiveFunc;
    }

    /// <summary>
    /// Do not allow <see cref="Item"/> to be null.
    /// This creates a mess when working with the tabs.
    /// <br/><br/>
    /// If one wishes to have a 'null' concept,
    /// then one should make a datatype that acts as a psuedo null.
    /// </summary>
    public T Item { get; }

    public override object UntypedItem => Item!;
    public override Type ItemType => typeof(T);
    public override Func<TabEntryNoType, string> GetDisplayNameFunc { get; }
    public override Action<TabEntryNoType> OnSetActiveFunc { get; }
}
