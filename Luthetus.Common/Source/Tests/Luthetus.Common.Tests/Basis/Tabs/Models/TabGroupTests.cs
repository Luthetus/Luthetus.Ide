using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.Tests.Basis.WatchWindows;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Tabs.Models;

/// <summary>
/// <see cref="TabGroup"/>
/// </summary>
public class TabGroupTests
{
    /// <summary>
    /// <see cref="TabGroup(Func{TabGroupLoadTabEntriesArgs, Task{TabGroupLoadTabEntriesOutput}}, RazorLib.Keys.Models.Key{TabGroup})"/>
    /// <br/>----<br/>
    /// <see cref="TabGroup.EntryList"/>
    /// <see cref="TabGroup.LoadEntryListAsync()"/>
    /// <see cref="TabGroup.ActiveEntryKey"/>
    /// <see cref="TabGroup.GetActiveEntryNoType()"/>
    /// <see cref="TabGroup.Key"/>
    /// </summary>
    [Fact]
    public async Task ConstructorAsync()
    {
        TabsTestsHelper.InitializeTabStateActionsTests(
            out _,
            out var redTabEntry,
            out var greenTabEntry,
            out var blueTabEntry,
            out _);

        var tabEntries = new TabEntryNoType[]
        {
            redTabEntry,
            greenTabEntry,
            blueTabEntry,
        }.ToImmutableList();

        // 'TabsTestsHelper.InitializeTabStateActionsTests' will provide a TabGroup instance but,
        // because this test is for the TabGroup constructor,
        // I'd like to explicitly invoke the constructor.
        var tabGroup = new TabGroup(
            loadTabEntriesArgs => Task.FromResult(new TabGroupLoadTabEntriesOutput(tabEntries)),
            Key<TabGroup>.NewKey());

        // Assert EntryList
        {
            Assert.Empty(tabGroup.EntryList);

            tabGroup = tabGroup with
            {
                EntryList = (await tabGroup.LoadEntryListAsync()).OutTabEntries
            };

            Assert.Equal(tabEntries, tabGroup.EntryList);
        }

        Assert.Equal(Key<TabEntryNoType>.Empty, tabGroup.ActiveEntryKey);

        // Ensure 'GetActiveEntryNoType()' is NOT just returning whatever item exists at
        // a hardcoded index.
        //
        // Do this by setting the ActiveEntryKey twice. Since the objects with
        // those keys are different, then the two objects should not be equal.
        {
            var itemIndexOne = tabGroup.EntryList[1];
            tabGroup = tabGroup with
            {
                ActiveEntryKey = itemIndexOne.TabEntryKey
            };
            
            Assert.Equal(itemIndexOne.TabEntryKey, tabGroup.ActiveEntryKey);
            var getActiveEntryNoTypeResult = tabGroup.GetActiveEntryNoType();
            Assert.Equal(itemIndexOne, getActiveEntryNoTypeResult);

            var itemIndexTwo = tabGroup.EntryList[2];
            tabGroup = tabGroup with
            {
                ActiveEntryKey = itemIndexTwo.TabEntryKey
            };

            Assert.Equal(itemIndexTwo.TabEntryKey, tabGroup.ActiveEntryKey);
            getActiveEntryNoTypeResult = tabGroup.GetActiveEntryNoType();
            Assert.Equal(itemIndexTwo, getActiveEntryNoTypeResult);
        }

        // Assert setting Key<TabEntryNoType>.Empty works properly
        {
            tabGroup = tabGroup with
            {
                ActiveEntryKey = Key<TabEntryNoType>.Empty
            };

            Assert.Equal(Key<TabEntryNoType>.Empty, tabGroup.ActiveEntryKey);
        }

        Assert.NotEqual(Key<TabGroup>.Empty, tabGroup.Key);

        // Make another tab entry, then assert that they have different keys
        {
            var otherTabGroup = new TabGroup(
                loadTabEntriesArgs => Task.FromResult(new TabGroupLoadTabEntriesOutput(tabEntries)),
                Key<TabGroup>.NewKey());

            Assert.NotEqual(otherTabGroup.Key, tabGroup.Key);
        }
    }
}
