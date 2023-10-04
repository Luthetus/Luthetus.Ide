using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Models;

/// <summary>
/// TODO: SphagettiCode - many 'tab blazor components' are written because there was
/// no generic 'tab blazor component' at the time. Those old specific 'tab blazor components'
/// should be replaced with this generic version (2023-09-19)
/// </summary>
public record TabGroup(

    Func<TabGroupLoadTabEntriesParameter, Task<TabGroupLoadTabEntriesOutput>> LoadEntryBagAsyncFunc)
{
    public TabGroup(
            Func<TabGroupLoadTabEntriesParameter, Task<TabGroupLoadTabEntriesOutput>> loadEntryBagAsyncFunc,
            Key<TabGroup> groupKey)
        : this(loadEntryBagAsyncFunc)
    {
        Key = groupKey;
    }

    public ImmutableList<TabEntryNoType> EntryBag { get; init; } = ImmutableList<TabEntryNoType>.Empty;
    public Key<TabEntryNoType> ActiveEntryKey { get; init; } = Key<TabEntryNoType>.Empty;
    public Key<TabGroup> Key { get; } = Key<TabGroup>.NewKey();

    public async Task<TabGroupLoadTabEntriesOutput> LoadEntryBagAsync()
    {
        var tabGroupLoadTabEntriesParameter = new TabGroupLoadTabEntriesParameter(EntryBag);

        return await LoadEntryBagAsyncFunc.Invoke(tabGroupLoadTabEntriesParameter);
    }

    public TabEntryNoType? GetActiveEntryNoType()
    {
        return EntryBag.SingleOrDefault(x => x.TabEntryKey == ActiveEntryKey);
    }
}
