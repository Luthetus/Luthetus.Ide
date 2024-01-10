using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.States;

public partial record TabState
{
    public record RegisterTabGroupAction(TabGroup TabGroup);
    public record DisposeTabGroupAction(Key<TabGroup> TabGroupKey);
    public record SetTabEntryListAction(Key<TabGroup> TabGroupKey, ImmutableList<TabEntryNoType> TabEntryList);
    public record SetActiveTabEntryKeyAction(Key<TabGroup> TabGroupKey, Key<TabEntryNoType> TabEntryKey);
}
