using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Models;

/// <summary>
/// TODO: SphagettiCode - many 'tab blazor components' are written because there was
/// no generic 'tab blazor component' at the time. Those old specific 'tab blazor components'
/// should be replaced with this generic version (2023-09-19)
/// </summary>
public record TabGroup(

    Func<TabGroupLoadTabEntriesArgs, Task<TabGroupLoadTabEntriesOutput>> LoadEntryListAsyncFunc)
{
    public TabGroup(
            Func<TabGroupLoadTabEntriesArgs, Task<TabGroupLoadTabEntriesOutput>> loadEntryListAsyncFunc,
            Key<TabGroup> groupKey)
        : this(loadEntryListAsyncFunc)
    {
        Key = groupKey;
    }

    public ImmutableList<TabEntryNoType> EntryList { get; init; } = ImmutableList<TabEntryNoType>.Empty;
    public Key<TabEntryNoType> ActiveEntryKey { get; init; } = Key<TabEntryNoType>.Empty;
    public Key<TabGroup> Key { get; } = Key<TabGroup>.NewKey();

    public async Task<TabGroupLoadTabEntriesOutput> LoadEntryListAsync()
    {
        var tabGroupLoadTabEntriesArgs = new TabGroupLoadTabEntriesArgs(EntryList);

        return await LoadEntryListAsyncFunc.Invoke(tabGroupLoadTabEntriesArgs).ConfigureAwait(false);
    }

    public TabEntryNoType? GetActiveEntryNoType()
    {
        return EntryList.SingleOrDefault(x => x.TabEntryKey == ActiveEntryKey);
    }
}
