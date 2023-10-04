using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public abstract record TabEntryNoType
{
    public abstract object UntypedItem { get; }
    public abstract Type ItemType { get; }
    public abstract Func<TabEntryNoType, string> GetDisplayNameFunc { get; }
    public abstract Action<TabEntryNoType> OnSetActiveFunc { get; }

    public Key<TabEntryNoType> TabEntryKey { get; } = Key<TabEntryNoType>.NewKey();
}
